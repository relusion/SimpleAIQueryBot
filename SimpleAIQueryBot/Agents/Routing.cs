#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed
using System.Text.Json;
using Microsoft.SemanticKernel;
using SimpleAIQueryBot.Tools;

namespace Agents
{
    public enum RoutingState
    {
        ContextRequired,
        NoContextRequired,
        Unknown
    }

    public class Routing
    {
        const string prompt = 
        """
        You are an expert at routing questions. Here are the available routes:
        - ContextRequired
        - NoContextRequired
        
        Use ContextRequired for questions that:
        - Require more context for a better answer
        - Are about recent events
        - Involve future predictions or forecasts
        - Ask about specific entities or events that likely need recent or detailed information

        Return the JSON with a single key "action" without any preamble or explanation. Always return only JSON data without any formatting or markdown.

        Examples:
        {"action":"ContextRequired"}
        {"action":"NoContextRequired"}
        Question to route: {{$question}}
        """;

        Kernel _kernel;

        public Routing()
        {
            _kernel = SemanticKernelConfig.CreateKernel();
        }

        public RoutingState Invoke(string userQuery)
        {
            var queryFunc = _kernel.CreateFunctionFromPrompt(prompt);
            var res = queryFunc.InvokeAsync(_kernel, new KernelArguments
            {
                ["question"] = userQuery
            }).Result;
            
            var json = res.GetValue<string>()!;
            var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(json);
            var actionvalue = "";
            if(dict == null || !dict.TryGetValue("action", out actionvalue))
            {
                throw new InvalidOperationException("Invalid response received from LLM: no action property returned :-/");
            }

            var routingState = RoutingState.Unknown;
            if(!Enum.TryParse<RoutingState>(actionvalue, out routingState))
            {
                throw new InvalidOperationException($"Invalid response received from LLM: Invalid action value: {actionvalue} :-/");
            }

            return routingState;
        }
    }
}

