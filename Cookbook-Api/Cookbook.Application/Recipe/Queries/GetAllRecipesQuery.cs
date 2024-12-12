using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cookbook.Application.Recipe.Queries;

public record GetAllRecipesQuery : IRequest<Result<IEnumerable<RecipeDto>>>;

public class GetAllRecipesQueryHandler : IRequestHandler<GetAllRecipesQuery, Result<IEnumerable<RecipeDto>>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<GetAllRecipesQueryHandler> _logger;

    public GetAllRecipesQueryHandler(IRecipeRepository recipeRepository, ILogger<GetAllRecipesQueryHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recipes = await _recipeRepository.GetAll(cancellationToken);
            return Result<IEnumerable<RecipeDto>>.Success(recipes.Select(RecipeDtoExtensions.FromEntity));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong while fetching recipes");
            return Result.Error(e.Message);
        }
    }
}