var builder = DistributedApplication.CreateBuilder(args);


var goolgeGeminiConfigurationApiKey = 
    builder.Configuration["GoogleGeminiConfiguration:ApiKey"] ?? 
    throw new InvalidOperationException("Google Gemini API key is not configured.");

var apiService = builder.AddProject<Projects.AuthorAssistant_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithEnvironment(callback => 
    {
        callback.EnvironmentVariables.Add("GoogleGeminiConfiguration:ApiKey", goolgeGeminiConfigurationApiKey);
    });

builder.AddProject<Projects.AuthorAssistant_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
