using System.Text.Json;
using Cookbook.Common.Json;

namespace Cookbook.Tests.Common.Json;

public class CustomJsonNamingPolicyTest
{
    [Fact]
    public void Serialize_ObjectWithDiacritics_ShouldSerializeToJsonWithoutDiacritics()
    {
        var obj = new CustomJsonObject(1, 2, 3, 4, 5);

        var actual = JsonSerializer.Serialize(obj, new JsonSerializerOptions { PropertyNamingPolicy = new CustomJsonNamingPolicy() });

        actual.ShouldBe("""{"Normal":1,"AIBorjan":2,"InnehallerA":3,"FortsatterMedO":4,"gemenIBorjan":5}""");
    }

    private record CustomJsonObject(int Normal, int ÅIBörjan, int InnehållerÄ, int FortsätterMedÖ, int gemenIBörjan);
}
