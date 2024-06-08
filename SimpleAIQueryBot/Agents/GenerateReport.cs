using Microsoft.SemanticKernel;
using SimpleAIQueryBot.Tools;

namespace SimpleAIQueryBot.Agents
{
    public class GenerateReport
    {
        const string prompt =
     """
     You are a helpful, smart, kind, and efficient AI assistant. You always fulfill the user's requests to the best of your ability. If web search context is available, strictly use the web search context to address the question. If the answer is not found within the provided context, clearly state that the information is unavailable. Ensure the response is concise but comprehensive. Strive for simplicity, clarity, and directness in your phrasing. Use a matter-of-fact tone, with fewer adjectives and a straightforward approach. Only reference material directly if it is included in the context.
     Question: {{$question}}
     
     Web Search Context: {{$context}}
     
     Answer:
    """;

        Kernel _kernel;

        public GenerateReport()
        {
            _kernel = SemanticKernelConfig.CreateKernel();
        }

        public string Invoke(QueryContext queryContext)
        {            
            KernelFunction queryFunc = _kernel.CreateFunctionFromPrompt(prompt);
            var res = queryFunc.InvokeAsync(_kernel, new KernelArguments
            {
                ["question"] = queryContext.Query,
                ["context"] = queryContext.Data,
            }).Result;
            var report = res.GetValue<string>()!;
            
            return report;
        }

    }
}