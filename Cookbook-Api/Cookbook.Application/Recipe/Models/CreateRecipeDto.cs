﻿namespace Cookbook.Application.Recipe.Models;

public record CreateRecipeDto(string Name, string Description, IEnumerable<string> Instructions, IEnumerable<IngredientDto> Ingredients);
