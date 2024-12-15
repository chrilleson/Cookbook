using Ardalis.Result;
using Ardalis.Result.AspNetCore;
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

        group.MapPost("/", async (IMediator mediator, RecipeDto recipeDto, CancellationToken cancellationToken) =>
            (await Result
                .Created(new CreateRecipeCommand(recipeDto))
                .BindAsync(x => mediator.Send(x, cancellationToken)))
            .ToMinimalApiResult())
            .Produces(StatusCodes.Status201Created);

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                (await Result
                    .Created(new GetAllRecipesQuery())
                    .BindAsync(x => mediator.Send(x, cancellationToken)))
                .ToMinimalApiResult())
            .Produces<IEnumerable<RecipeDto>>();

        group.MapGet("/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            (await Result
                .Created(new GetRecipeByIdQuery(id))
                .BindAsync(x => mediator.Send(x, cancellationToken)))
            .ToMinimalApiResult())
            .Produces<RecipeDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            (await Result
                .Created(new RemoveRecipeCommand(id))
                .BindAsync(x => mediator.Send(x, cancellationToken))
            ).ToMinimalApiResult())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}