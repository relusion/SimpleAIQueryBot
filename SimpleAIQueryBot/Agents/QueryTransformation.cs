using System.Text.Json;
using Microsoft.SemanticKernel;
using SimpleAIQueryBot.Tools;

namespace SimpleAIQueryBot.Agents
{
    public class QueryTransformation
    {
        const string prompt = """
    You are an expert at web search. Based on the user's question, convert the main inquiry into a clear and concise search expression for Google. Focus on keywords and phrases that directly relate to the topic, avoiding unnecessary words. Include variations or synonyms if applicable to improve search results.
    For example:
    User Input: "How can I implement Kubernetes in an Azure environment with a focus on security best practices?"
    
    Web Search Expression:
    "Implement Kubernetes in Azure security best practices"
    
    Return the JSON with single key "search_query" with no premable or explanation.
    You always return only JSON data without any formatting or markdown.
    Examples:
    {"search_query":"Implement Kubernetes in Azure security best practices"}    
    
    User Query: {{$query}}
    """;

        Kernel _kernel;

        public QueryTransformation()
        {
            _kernel = SemanticKernelConfig.CreateKernel();

        }

        public string Invoke(string userQuery)
        {
            var queryFunc = _kernel.CreateFunctionFromPrompt(prompt);
            var res = queryFunc.InvokeAsync(_kernel, new KernelArguments
            {
                ["query"] = userQuery
            }).Result;
            var json = res.GetValue<string>()!;
            
            var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(json);
            var searchQueryValue = "";
            if(dict == null || !dict.TryGetValue("search_query", out searchQueryValue))
            {
                throw new InvalidOperationException("Invalid response received from LLM: no search_query property returned :-/");
            }

            return searchQueryValue;
        }

    }
}