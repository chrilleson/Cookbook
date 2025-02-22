var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "cookbook")
    .WithEnvironment("PGDATA", "/var/lib/postgresql/data/pgdata")
    .WithDataBindMount(source: "../../data/postgres")
    .WithEndpoint(name: "postgres", port: 54320, targetPort: 5432, isExternal: true)
    .AddDatabase("cookbook");

var api = builder.AddProject<Projects.Cookbook_Api>("api")
    .WaitFor(postgres)
    .WithReference(postgres);

await builder.Build().RunAsync();