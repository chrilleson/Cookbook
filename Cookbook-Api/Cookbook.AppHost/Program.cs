var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "cookbook")
    .WithEnvironment("PGDATA", "/var/lib/postgresql/data/pgdata")
    .WithDataBindMount(source: "../../data/postgres")
    .WithEndpoint(name: "postgres", port: 54320, targetPort: 5432, isExternal: true)
    .WithLifetime(ContainerLifetime.Session);

var cookbookDb = postgres.AddDatabase("cookbook");

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .WithDataBindMount(source: "../../data/redis")
    .WithLifetime(ContainerLifetime.Session);

var api = builder.AddProject<Projects.Cookbook_Api>("api")
    .WaitFor(cookbookDb)
    .WithReference(cookbookDb)
    .WithReference(redis);

await builder.Build().RunAsync();