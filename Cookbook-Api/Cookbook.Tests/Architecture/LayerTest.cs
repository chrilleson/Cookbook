using System.Reflection;
using Cookbook.Api.Endpoints;
using Cookbook.Application;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Infrastructure.Persistence.Repositories;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetArchTest.Rules;

namespace Cookbook.Tests.Architecture;

public class LayerTest
{
    private static readonly Assembly DomainAssembly = typeof(Recipe).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationExtensions).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(PersistenceExtensions).Assembly;
    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void Domain_ShouldNot_ReferenceApplication()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Domain_ShouldNot_ReferenceInfrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Domain_ShouldNot_ReferenceApi()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApiAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_ShouldNot_ReferenceApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(ApiAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_ShouldNot_ReferenceInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_Should_ReferenceDomain()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Repsoitory")
            .Should()
            .HaveDependencyOn(DomainAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNot_ReferenceApi()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(ApiAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Infrastructure_Should_ReferenceDomain()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .HaveName(nameof(AppDbContext))
            .Should()
            .HaveDependencyOn(DomainAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Infrastructure_Should_ReferenceApplication()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .HaveName(nameof(UnitOfWork), nameof(RecipeRepository))
            .Should()
            .HaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Api_Should_ReferenceInfrastructure()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .HaveName(nameof(Program))
            .Should()
            .HaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Api_Should_ReferenceDomain()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .HaveName(nameof(Program))
            .Should()
            .HaveDependencyOn(DomainAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Api_Should_ReferenceApplication()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ImplementInterface(typeof(IEndpoint))
            .Should()
            .HaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue();
    }
}
