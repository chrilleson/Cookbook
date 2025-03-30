using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cookbook.Infrastructure.Persistence.Configurations.Comparers;

public class JsonArrayComparer<T> : ValueComparer<IReadOnlyCollection<T>> where T : class
{
    public JsonArrayComparer() : base(
        (c1, c2) => c1.SequenceEqual(c2),
        c => c.GetHashCode(),
        c => c.ToList())
    {
    }
}
