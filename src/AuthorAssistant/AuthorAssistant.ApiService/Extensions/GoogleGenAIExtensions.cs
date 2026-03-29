using AuthorAssistant.Services.GoogleGemini;
using AuthorAssistant.Services.NanoBanana;
using AuthorAssistant.Services.Veo;

namespace AuthorAssistant.ApiService.Extensions
{
    public static class GoogleGenAIExtensions
    {
        public static IServiceCollection AddGoogleGenAI(this IServiceCollection services, IConfiguration configuration)
        {
            GoogleGeminiConfiguration googleGeminiConfiguration =
                configuration.GetSection("GoogleGeminiConfiguration")
                .Get<GoogleGeminiConfiguration>() ??
                throw new InvalidOperationException("Google Gemini configuration is missing.");
            services.AddSingleton(googleGeminiConfiguration);
            services.AddKeyedTransient<Google.GenAI.Client>(serviceKey: "GoogleGeminiServiceGenAIClient",
                (sp, key) =>
                {
                    var config = sp.GetRequiredService<GoogleGeminiConfiguration>();
                    return new Google.GenAI.Client(apiKey: config.ApiKey);
                });
            services.AddKeyedTransient<Google.GenAI.Client>(serviceKey: "NanoBananaServiceGenAIClient",
                (sp, key) => 
                {
                    var config = sp.GetRequiredService<GoogleGeminiConfiguration>();
                    return new Google.GenAI.Client(apiKey: config.ApiKey,
                        httpOptions: new Google.GenAI.Types.HttpOptions()
                        {
                            Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds
                        });
                });
            services.AddKeyedTransient<Google.GenAI.Client>(serviceKey: "VeoServiceGenAIClient",
                (sp, key) =>
                {
                    var config = sp.GetRequiredService<GoogleGeminiConfiguration>();
                    return new Google.GenAI.Client(apiKey: config.ApiKey,
                        httpOptions: new Google.GenAI.Types.HttpOptions()
                        {
                            Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds
                        });
                });
            services.AddTransient<INanoBananaService, NanoBananaService>(sp=>
            {
                var genAIClient = sp.GetRequiredKeyedService<Google.GenAI.Client>("NanoBananaServiceGenAIClient");
                var logger = sp.GetRequiredService<ILogger<NanoBananaService>>();
                return new NanoBananaService(logger, genAIClient);
            });
            services.AddTransient<IGoogleGeminiService, GoogleGeminiService>(sp => 
            {
                var genAIClient = sp.GetRequiredKeyedService<Google.GenAI.Client>("GoogleGeminiServiceGenAIClient");
                var logger = sp.GetRequiredService<ILogger<GoogleGeminiService>>();
                return new GoogleGeminiService(logger, genAIClient);
            });
            services.AddTransient<IVeoService, VeoService>(sp =>
            {
                var genAIClient = sp.GetRequiredKeyedService<Google.GenAI.Client>("VeoServiceGenAIClient");
                var logger = sp.GetRequiredService<ILogger<VeoService>>();
                return new VeoService(logger, genAIClient);
            });
            return services;
        }
    }
}
