using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Repositories;
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
            var recipe = await _recipeRepository.GetById(request.Id, cancellationToken);
            return recipe is not null
                ? Result.Success(recipe.FromEntity())
                : Result<RecipeDto>.NotFound();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not find recipe with id: {Id}", request.Id);
            return Result<RecipeDto>.Error("Could not find recipe");
        }
    }
}