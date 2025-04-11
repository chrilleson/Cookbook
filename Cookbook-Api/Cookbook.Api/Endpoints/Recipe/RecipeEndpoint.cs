using Ardalis.Result;
using Cookbook.Api.Constants;
using Cookbook.Api.Extensions;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Cookbook.Api.Endpoints.Recipe;

public class RecipeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/recipes")
            .WithTags("Recipes")
            .ProducesProblem(statusCode: StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError);

        group.MapGet("/list", async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new GetAllRecipesQuery())
                    .BindAsync(x => mediator.Send(x, cancellationToken));

                return result.ToMinimalApiResult();
            })
            .CacheOutput(RecipeConstants.CacheTagListRecipes)
            .Produces<IEnumerable<RecipeDto>>();

        group.MapGet("/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new GetRecipeByIdQuery(id))
                    .BindAsync(x => mediator.Send(x, cancellationToken));

                return result switch
                {
                    { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
                    _ => result.ToMinimalApiResult(),
                };
            })
            .CacheOutput(RecipeConstants.CacheTagRecipeById)
            .Produces<RecipeDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPost("/", async (CreateRecipeDto recipeDto, IMediator mediator, IOutputCacheStore cacheStore, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new CreateRecipeCommand(recipeDto))
                    .BindAsync(x => mediator.Send(x, cancellationToken));

                await InvalidateCache(result, RecipeConstants.CacheTagListRecipes, cacheStore, cancellationToken);


                return result switch
                {
                    { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
                    _ => result.ToMinimalApiResult(),
                };
            })
            .Produces<RecipeDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapDelete("/{id:int}", async (int id, IMediator mediator, IOutputCacheStore cacheStore, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new RemoveRecipeCommand(id))
                    .BindAsync(x => mediator.Send(x, cancellationToken));

                await InvalidateCache(result, RecipeConstants.CacheTagRecipeById, cacheStore, cancellationToken);


                return result switch
                {
                    { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
                    _ => result.ToMinimalApiResult(),
                };
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPut("/{id:int}", async (int id, [FromBody] UpdateRecipeDto recipeDto, IMediator mediator, IOutputCacheStore cacheStore, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new UpdateRecipeCommand(id, recipeDto))
                    .BindAsync(x => mediator.Send(x, cancellationToken));

                await InvalidateCache(result, RecipeConstants.CacheTagRecipeById, cacheStore, cancellationToken);


                return result switch
                {
                    { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
                    _ => result.ToMinimalApiResult(),
                };
            })
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task InvalidateCache<T>(Result<T> result, string cacheTag, IOutputCacheStore cacheStore, CancellationToken cancellationToken)
    {
        if (result.IsSuccess)
        {
            await cacheStore.EvictByTagAsync(cacheTag, cancellationToken);
        }
    }
}
