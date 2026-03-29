using AuthorAssistant.Services.GoogleGemini;

namespace AuthorAssistant.ApiService.Extensions
{
    public static class GoogleGeminiExtensions
    {
        public static IServiceCollection AddGoogleGemini(this IServiceCollection services, IConfiguration configuration)
        {
            GoogleGeminiConfiguration googleGeminiConfiguration =
                configuration.GetSection("GoogleGeminiConfiguration")
                .Get<GoogleGeminiConfiguration>() ??
                throw new InvalidOperationException("Google Gemini configuration is missing.");
            services.AddSingleton(googleGeminiConfiguration);
            services.AddTransient<Google.GenAI.Client>(sp =>
            {
                var config = sp.GetRequiredService<GoogleGeminiConfiguration>();
                return new Google.GenAI.Client(apiKey: config.ApiKey,
                    httpOptions:new Google.GenAI.Types.HttpOptions()
                    {
                        Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds
                    });
            });
            services.AddTransient<IGoogleGeminiService, GoogleGeminiService>();
            return services;
        }
    }
}
