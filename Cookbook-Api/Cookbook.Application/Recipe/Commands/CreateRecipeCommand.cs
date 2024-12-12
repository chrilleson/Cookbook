using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Unit = MediatR.Unit;

namespace Cookbook.Application.Recipe.Commands;

public record CreateRecipeCommand(RecipeDto Recipe) : IRequest<Result<Unit>>;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Result<Unit>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRecipeCommand> _logger;

    public CreateRecipeCommandHandler(IRecipeRepository recipeRepository, IUnitOfWork unitOfWork, ILogger<CreateRecipeCommand> logger)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = request.Recipe.ToEntity();
            await _recipeRepository.Add(recipe, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.NoContent();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while creating new recipe");
            return Result<Unit>.Error(e.Message);
        }
    }
}