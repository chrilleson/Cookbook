using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Services;
using Cookbook.Domain.Recipe.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cookbook.Application.Recipe.Queries;

public record GetRecipeByIdQuery(int Id) : IRequest<Result<RecipeDto>>;

public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, Result<RecipeDto>>
{
    private readonly IRecipeService _recipeService;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<GetRecipeByIdQueryHandler> _logger;

    public GetRecipeByIdQueryHandler(IRecipeRepository recipeRepository, ILogger<GetRecipeByIdQueryHandler> logger, IRecipeService recipeService)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
        _recipeService = recipeService;
    }

    public async Task<Result<RecipeDto>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // var recipe = await _recipeRepository.GetById(request.Id, cancellationToken);
            var recipe = await _recipeService.GetByIdAsync(request.Id, cancellationToken);
            return recipe is not null
                ? Result.Success(recipe, "Recipe found")
                : Result.NotFound("Recipe not found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not find recipe with id: {Id}", request.Id);
            return Result<RecipeDto>.Error("Something went wrong while fetching recipe");
        }
    }
}
