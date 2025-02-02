using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Cookbook.Application.Recipe.Commands;

public record UpdateRecipeCommand(RecipeDto Recipe) : IRequest<Result>;

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
            var existingRecipe = await _recipeRepository.GetById(request.Recipe.Id, cancellationToken);
            if (existingRecipe is null) return Result.NotFound();

            var updatedRecipe = existingRecipe.Update(request.Recipe);

            _recipeRepository.Update(updatedRecipe);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("Recipe updated");
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