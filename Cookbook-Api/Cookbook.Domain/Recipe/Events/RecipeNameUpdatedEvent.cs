using Cookbook.Domain.Shared.Events;

namespace Cookbook.Domain.Recipe.Events;

public class RecipeNameUpdatedEvent : DomainEvent
{
    public Entities.Recipe Recipe { get; }

    public RecipeNameUpdatedEvent(Entities.Recipe recipe)
    {
        Recipe = recipe;
    }
}
