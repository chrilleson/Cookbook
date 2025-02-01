using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new GetAllRecipesQuery())
                    .BindAsync(x => mediator.Send(x, cancellationToken))
                    .MapAsync(x => x);

                return result.ToMinimalApiResult();
            })
            .Produces<IEnumerable<RecipeDto>>();

        group.MapPost("/", async (IMediator mediator, RecipeDto recipeDto, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new CreateRecipeCommand(recipeDto))
                    .BindAsync(x => mediator.Send(x, cancellationToken))
                    .MapAsync(x => x);

                return result switch
                {
                    { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
                    _ => result.ToMinimalApiResult(),
                };
            })
            .Produces<RecipeDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();


        group.MapGet("/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new GetRecipeByIdQuery(id))
                    .BindAsync(x => mediator.Send(x, cancellationToken))
                    .MapAsync(x => x);

                return result.ToMinimalApiResult();
            })
            .Produces<Result<RecipeDto>>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .BindAsync(_ => mediator.Send(new RemoveRecipeCommand(id), cancellationToken))
                    .MapAsync(x => x);

                return result switch
                {
                    { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
                    _ => result.ToMinimalApiResult(),
                };
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPut("/{id:int}", async (IMediator mediator, int id, [FromBody] RecipeDto recipeDto, CancellationToken cancellationToken) =>
            {
                var result = await new Result()
                    .Map(() => new UpdateRecipeCommand(recipeDto))
                    .BindAsync(x => mediator.Send(x, cancellationToken));

                return result.ToMinimalApiResult();
            })
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}