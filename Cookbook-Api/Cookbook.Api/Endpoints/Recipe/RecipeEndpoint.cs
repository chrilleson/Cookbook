using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Queries;
using MediatR;

namespace Cookbook.Api.Endpoints.Recipe;

public class RecipeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/recipes").WithTags("Recipes");

        group.MapPost("", async (IMediator mediator, RecipeDto recipeDto, CancellationToken cancellationToken) => (await Result
                .Created(new CreateRecipeCommand(recipeDto))
                .BindAsync(x => mediator.Send(x, cancellationToken)))
            .ToMinimalApiResult());

        group.MapGet("", async (IMediator mediator, CancellationToken cancellationToken) =>
            (await Result
                .Created(new GetAllRecipesQuery())
                .BindAsync(x => mediator.Send(x, cancellationToken)))
            .ToMinimalApiResult());
    }
}