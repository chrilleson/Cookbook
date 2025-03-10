﻿using Ardalis.Result;
using Cookbook.Application.Extensions;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Repositories;
using Cookbook.Application.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Cookbook.Application.Recipe.Commands;

public record CreateRecipeCommand(CreateRecipeDto Recipe) : IRequest<Result<RecipeDto>>;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Result<RecipeDto>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRecipeCommandHandler> _logger;

    public CreateRecipeCommandHandler(IRecipeRepository recipeRepository, IUnitOfWork unitOfWork, ILogger<CreateRecipeCommandHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RecipeDto>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = request.Recipe.ToEntity();
            await _recipeRepository.Add(recipe, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Created(recipe.ToDto(), RecipeRoutes.FormatRoute(RecipeRoutes.GetRecipe, recipe.Id));
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: "23505" })
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Recipe already exists");
            return Result.Conflict("Recipe already exists");
        }
        catch (DbUpdateException e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while creating new recipe");
            return Result.Error(e.Message);
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            _logger.LogError(e, "Something went wrong while creating new recipe");
            return Result.Error(e.Message);
        }
    }
}
