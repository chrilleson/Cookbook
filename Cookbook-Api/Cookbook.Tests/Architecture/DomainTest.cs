using System.Reflection;
using Cookbook.Domain.Recipe;
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
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
