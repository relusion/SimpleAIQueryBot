#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed
using Microsoft.SemanticKernel;

namespace SimpleAIQueryBot.Tools
{
    public class SemanticKernelConfig
    {
        public static Kernel CreateKernel()
        {
            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(600)
            };

            var builder = Kernel.CreateBuilder();

            var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var openAIEndpoint = Environment.GetEnvironmentVariable("OPENAI_API_ENDPOINT");
            var openAIModelId = Environment.GetEnvironmentVariable("OPENAI_API_MODEL_ID");

            if (openAIEndpoint == null)
            {
                throw new ArgumentNullException("OPENAI_API_ENDPOINT", "The OPENAI_API_ENDPOINT environment variable is missing.");
            }

            if (openAIModelId == null)
            {
                throw new ArgumentNullException("OPENAI_API_MODEL_ID", "The OPENAI_API_MODEL_ID environment variable is missing.");
            }

            builder.AddOpenAIChatCompletion(
                modelId: openAIModelId,
                apiKey: openAIKey,
                endpoint: new Uri(openAIEndpoint),
                httpClient: httpClient
            );

            return builder.Build();
        }
        
    }
}