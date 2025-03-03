var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "cookbook")
    .WithEnvironment("PGDATA", "/var/lib/postgresql/data/pgdata")
    .WithVolume("postgres-data", "/var/lib/postgresql/data/pgdata")
    .WithEndpoint(name: "postgres", port: 54320, targetPort: 5432, isExternal: true)
    .WithLifetime(ContainerLifetime.Session)
    .WithContainerName("cookbook-postgres");

var cookbookDb = postgres.AddDatabase("cookbook");

var redis = builder.AddRedis("redis")
    .WithVolume("redis-data", "/data")
    .WithLifetime(ContainerLifetime.Session)
    .WithContainerName("cookbook-redis")
    .WithRedisCommander(x =>
    {
        x.WithVolume("redis-commander-data", "/data");
        x.WithLifetime(ContainerLifetime.Session);
        x.WithContainerName("cookbook-redis-commander");
    });

var api = builder.AddProject<Projects.Cookbook_Api>("api")
    .WaitFor(cookbookDb)
    .WithReference(cookbookDb)
    .WithReference(redis);

await builder.Build().RunAsync();