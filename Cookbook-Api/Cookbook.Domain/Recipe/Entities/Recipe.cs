using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using Cookbook.Domain.Recipe.Events;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Domain.Shared.Events;
using Cookbook.Domain.Shared.Exceptions;

namespace Cookbook.Domain.Recipe.Entities;

public sealed class Recipe
{
    private readonly List<Instruction> _instructions = [];
    private readonly List<Ingredient> _ingredients = [];
    private readonly List<IDomainEvent> _domainEvents = [];

    public RecipeId Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Instruction> Instructions => _instructions.AsReadOnly();
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();

    [Timestamp]
    public byte[]? RowVersion { get; init; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public Recipe(RecipeId id, string name, string? description = null)
    {
        Id = Guard.Against.Null(id, nameof(id));
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description;

        AddDomainEvent(new RecipeCreatedEvent(this));
    }

    public void UpdateName(string name)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        AddDomainEvent(new RecipeNameUpdatedEvent(this));
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        AddDomainEvent(new RecipeUpdatedEvent(this));
    }

    public void AddInstruction(string instructionText)
    {
        var stepNumber = _instructions.Count + 1;
        _instructions.Add(new Instruction(stepNumber, instructionText));
    }

    public void UpdateInstruction(int stepNumber, string instructionText)
    {
        var instruction = _instructions.FirstOrDefault(i => i.StepNumber == stepNumber);
        if (instruction is null)
        {
            throw new DomainException($"Instruction step {stepNumber} not found");
        }

        instruction.UpdateText(instructionText);
    }

    public void ReorderInstructions(Dictionary<int, int> reordering)
    {
        if (_instructions.Count != reordering.Count)
        {
            throw new DomainException("Reordering must include all instructions");
        }

        foreach (var instruction in _instructions)
        {
            if (reordering.TryGetValue(instruction.StepNumber, out var newStepNumber))
            {
                instruction.UpdateStepNumber(newStepNumber);
            }
            else
            {
                throw new DomainException($"Instruction step {instruction.StepNumber} missing from reordering");
            }
        }

        _instructions.Sort((a, b) => a.StepNumber.CompareTo(b.StepNumber));
    }

    public void ClearInstructions() => _instructions.Clear();

    public void AddIngredient(string name, double amount, MeasurementUnit unit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Ingredient name cannot be empty");
        }

        if (amount <= 0)
        {
            throw new DomainException("Ingredient amount must be positive");
        }

        var ingredient = new Ingredient(name, new Quantity(amount, unit));
        _ingredients.Add(ingredient);
    }

    public void RemoveIngredient(string name)
    {
        var ingredient = _ingredients.FirstOrDefault(i =>
            i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (ingredient == null)
        {
            throw new DomainException($"Ingredient {name} not found");
        }

        _ingredients.Remove(ingredient);
    }

    public void ClearIngredients() => _ingredients.Clear();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
