var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AuthorAssistant_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AuthorAssistant_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
