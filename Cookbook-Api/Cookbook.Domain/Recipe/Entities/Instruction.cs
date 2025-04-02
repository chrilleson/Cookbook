using Ardalis.GuardClauses;

namespace Cookbook.Domain.Recipe.Entities;

public sealed class Instruction
{
    public int StepNumber { get; private set; }
    public string Text { get; private set; }

    public Instruction(int stepNumber, string text)
    {
        StepNumber = Guard.Against.Negative(stepNumber, nameof(stepNumber));
        Text = Guard.Against.NullOrWhiteSpace(text, nameof(text));
    }

    public void UpdateText(string text) =>
        Text = Guard.Against.NullOrWhiteSpace(text, nameof(text));

    public void UpdateStepNumber(int stepNumber) =>
        StepNumber = Guard.Against.Negative(stepNumber, nameof(stepNumber));
}
