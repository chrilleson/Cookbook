using Ardalis.Result;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.UnitOfWork;
using Cookbook.Domain.Recipe.Repositories;
using Cookbook.Domain.Recipe.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Cookbook.Application.Recipe.Commands;

public record UpdateRecipeCommand(int Id, UpdateRecipeDto Recipe) : IRequest<Result>;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, Result>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRecipeCommandHandler> _logger;

    public UpdateRecipeCommandHandler(IRecipeRepository recipeRepository, IUnitOfWork unitOfWork, ILogger<UpdateRecipeCommandHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipeRepository.GetById(new RecipeId(request.Id), cancellationToken);
            if (recipe is null)
            {
                return Result.NotFound();
            }

            if (!string.IsNullOrEmpty(request.Recipe.Name))
            {
                recipe.UpdateName(request.Recipe.Name);
            }

            recipe.UpdateDescription(request.Recipe.Description);

            if (request.Recipe.Instructions != null)
            {
                recipe.ClearInstructions();

                foreach (var instruction in request.Recipe.Instructions)
                {
                    recipe.AddInstruction(instruction);
                }
            }

            if (request.Recipe.Ingredients != null)
            {
                recipe.ClearIngredients();

                foreach (var ingredientDto in request.Recipe.Ingredients)
                {
                    var unit = ingredientDto switch
                    {
                        { Unit.Fluid: not null } => MeasurementUnit.Fluid(ingredientDto.Unit.Fluid.Value),
                        { Unit.Weight: not null } => MeasurementUnit.Weight(ingredientDto.Unit.Weight.Value),
                        { Unit.Piece: not null } => MeasurementUnit.Piece(),
                        _ => throw new InvalidOperationException("Invalid unit")
                    };

                    recipe.AddIngredient(ingredientDto.Name, ingredientDto.Amount, unit);
                }
            }

            _recipeRepository.Update(recipe);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("Recipe updated");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _unitOfWork.Rollback();
            _logger.LogError(ex, "Concurrency conflict while updating recipe {RecipeId}", request.Id);

            // Get the current database values
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

            return Result.Error(databaseValues is null
                ? "The recipe has been deleted by another user."
                : "The recipe was modified by another user. Please refresh and try again."
            );
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: "23505" })
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Error updating recipe");
            return Result.Conflict("Recipe already exists");
        }
        catch (DbUpdateException e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Error updating recipe");
            return Result.Error(e.Message);
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Error updating recipe");
            return Result.Error(e.Message);
        }
    }
}
