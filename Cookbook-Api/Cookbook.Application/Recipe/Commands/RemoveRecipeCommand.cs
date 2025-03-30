using Ardalis.Result;
using Cookbook.Application.UnitOfWork;
using Cookbook.Domain.Recipe.Repositories;
using Cookbook.Domain.Recipe.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cookbook.Application.Recipe.Commands;

public record RemoveRecipeCommand(int Id) : IRequest<Result<Unit>>;

public class RemoveRecipeCommandHandler : IRequestHandler<RemoveRecipeCommand, Result<Unit>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveRecipeCommandHandler> _logger;

    public RemoveRecipeCommandHandler(IRecipeRepository recipeRepository, IUnitOfWork unitOfWork, ILogger<RemoveRecipeCommandHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RemoveRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipeRepository.GetById(new RecipeId(request.Id), cancellationToken);
            if (recipe is null)
            {
                return Result<Unit>.NotFound();
            }

            _recipeRepository.Remove(recipe);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.NoContent();
        }
        catch (DbUpdateException e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while removing recipe. Id: {Id}", request.Id);
            return Result.Error(e.Message);
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while removing recipe. Id: {Id}", request.Id);
            return Result<Unit>.Error(e.Message);
        }
    }
}
