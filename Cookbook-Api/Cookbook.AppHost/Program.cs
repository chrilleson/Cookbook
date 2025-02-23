var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "cookbook")
    .WithEnvironment("PGDATA", "/var/lib/postgresql/data/pgdata")
    .WithDataBindMount(source: "../../data/postgres")
    .WithEndpoint(name: "postgres", port: 54320, targetPort: 5432, isExternal: true)
    .AddDatabase("cookbook");

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .WithDataBindMount(source: "../../data/redis");

var api = builder.AddProject<Projects.Cookbook_Api>("api")
    .WaitFor(postgres)
    .WithReference(postgres)
    .WithReference(redis);

await builder.Build().RunAsync();