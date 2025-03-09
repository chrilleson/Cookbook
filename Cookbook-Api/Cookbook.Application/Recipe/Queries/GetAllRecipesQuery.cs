using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Services;
using Cookbook.Domain.Recipe.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cookbook.Application.Recipe.Queries;

public record GetAllRecipesQuery : IRequest<Result<IEnumerable<RecipeDto>>>;

public class GetAllRecipesQueryHandler : IRequestHandler<GetAllRecipesQuery, Result<IEnumerable<RecipeDto>>>
{
    private readonly IRecipeService _recipeService;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<GetAllRecipesQueryHandler> _logger;

    public GetAllRecipesQueryHandler(IRecipeRepository recipeRepository, ILogger<GetAllRecipesQueryHandler> logger, IRecipeService recipeService)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
        _recipeService = recipeService;
    }

    public async Task<Result<IEnumerable<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // var recipes = await _recipeRepository.GetAll(cancellationToken);
            var recipes = await _recipeService.GetAllAsync(cancellationToken);
            return Result.Success(recipes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong while fetching recipes");
            return Result.Error(e.Message);
        }
    }
}
