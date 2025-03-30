using Ardalis.GuardClauses;

namespace Cookbook.Domain.Recipe.Entities;

public class Instruction
{
    public int StepNumber { get; private set; }
    public string Text { get; private set; }

    private Instruction() { }

    public Instruction(int stepNumber, string text)
    {
        StepNumber = Guard.Against.Negative(stepNumber, exceptionCreator: () => new ArgumentException("Step number must be positive", nameof(stepNumber)));
        Text = Guard.Against.NullOrWhiteSpace(text, exceptionCreator: () => new ArgumentException("Instruction text cannot be empty", nameof(text)));
    }

    public void UpdateText(string text) =>
        Text = Guard.Against.NullOrWhiteSpace(text, exceptionCreator: () => new ArgumentException("Instruction text cannot be empty", nameof(text)));

    public void UpdateStepNumber(int stepNumber) =>
        StepNumber = Guard.Against.NegativeOrZero(stepNumber, exceptionCreator: () => new ArgumentException("Step number must be positive", nameof(stepNumber)));
}
