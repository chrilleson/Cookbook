using Cookbook.Domain.Shared.Events;

namespace Cookbook.Domain.Recipe.Events;

public class RecipeCreatedEvent : DomainEvent
{
    public Entities.Recipe Recipe { get; }

    public RecipeCreatedEvent(Entities.Recipe recipe)
    {
        Recipe = recipe;
    }
}
