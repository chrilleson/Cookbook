using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe.Repositories;
using Cookbook.Domain.Recipe.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cookbook.Application.Recipe.Queries;

public record GetRecipeByIdQuery(int Id) : IRequest<Result<RecipeDto>>;

public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, Result<RecipeDto>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<GetRecipeByIdQueryHandler> _logger;

    public GetRecipeByIdQueryHandler(IRecipeRepository recipeRepository, ILogger<GetRecipeByIdQueryHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }

    public async Task<Result<RecipeDto>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipeRepository.GetById(new RecipeId(request.Id), cancellationToken);
            return recipe is not null
                ? Result.Success(recipe.MapToDto(), "Recipe found")
                : Result.NotFound("Recipe not found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not find recipe with id: {Id}", request.Id);
            return Result<RecipeDto>.Error("Something went wrong while fetching recipe");
        }
    }
}
