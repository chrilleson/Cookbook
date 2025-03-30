using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Services;
using Cookbook.Application.UnitOfWork;
using Cookbook.Domain.Recipe.Repositories;
using Cookbook.Domain.Recipe.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Cookbook.Application.Recipe.Commands;

public record CreateRecipeCommand(CreateRecipeDto Recipe) : IRequest<Result<RecipeDto>>;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Result<RecipeDto>>
{
    private readonly IIdGenerator _idGenerator;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRecipeCommandHandler> _logger;

    public CreateRecipeCommandHandler(IIdGenerator idGenerator, IRecipeRepository recipeRepository, IUnitOfWork unitOfWork, ILogger<CreateRecipeCommandHandler> logger)
    {
        _idGenerator = idGenerator;
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RecipeDto>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var id = new RecipeId(_idGenerator.GenerateNextId());
            var recipe = new Domain.Recipe.Entities.Recipe(id, request.Recipe.Name, request.Recipe.Description);

            foreach (var instruction in request.Recipe.Instructions)
            {
                recipe.AddInstruction(instruction);
            }

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

            await _recipeRepository.Add(recipe, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Created(recipe.MapToDto(), RecipeRoutes.FormatRoute(RecipeRoutes.GetRecipe, recipe.Id));
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: "23505" })
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Recipe already exists");
            return Result.Conflict("Recipe already exists");
        }
        catch (DbUpdateException e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while creating new recipe");
            return Result.Error(e.Message);
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while creating new recipe");
            return Result.Error(e.Message);
        }
    }
}
