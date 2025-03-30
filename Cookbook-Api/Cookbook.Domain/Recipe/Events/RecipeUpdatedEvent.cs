using Cookbook.Domain.Shared.Events;

namespace Cookbook.Domain.Recipe.Events;

public class RecipeUpdatedEvent : DomainEvent
{
    public Entities.Recipe Recipe { get; }

    public RecipeUpdatedEvent(Entities.Recipe recipe)
    {
        Recipe = recipe;
    }
}
