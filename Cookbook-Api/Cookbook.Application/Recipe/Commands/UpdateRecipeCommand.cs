using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Repositories;
using Cookbook.Application.UnitOfWork;
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
            var existingRecipe = await _recipeRepository.GetById(request.Id, cancellationToken);
            if (existingRecipe is null) return Result.NotFound();

            var updatedRecipe = existingRecipe.Update(request.Recipe);
            updatedRecipe.RowVersion = request.Recipe.RowVersion;

            _recipeRepository.Update(updatedRecipe);
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