namespace Cookbook.Domain.Recipe.Entities;

public class Instruction
{
    public int StepNumber { get; private set; }
    public string Text { get; private set; }

    // For EF Core
    private Instruction() { }

    public Instruction(int stepNumber, string text)
    {
        if (stepNumber <= 0)
            throw new ArgumentException("Step number must be positive", nameof(stepNumber));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Instruction text cannot be empty", nameof(text));

        StepNumber = stepNumber;
        Text = text;
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Instruction text cannot be empty", nameof(text));

        Text = text;
    }

    public void UpdateStepNumber(int stepNumber)
    {
        if (stepNumber <= 0)
            throw new ArgumentException("Step number must be positive", nameof(stepNumber));

        StepNumber = stepNumber;
    }
}
