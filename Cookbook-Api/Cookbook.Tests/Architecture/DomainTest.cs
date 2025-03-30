using System.Reflection;
using Cookbook.Domain.Recipe.Entities;
using NetArchTest.Rules;

namespace Cookbook.Tests.Architecture;

public class DomainTest
{
    private static readonly Assembly DomainAssembly = typeof(Recipe).Assembly;

    [Fact]
    public void DomainClasses_Should_BeSealed()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ArePublic().And().AreClasses().And().DoNotHaveNameMatching("MeasurementUnit")
            .Should()
            .BeSealed().Or().BeAbstract()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
