using System.Diagnostics;
using Agents;
using SimpleAIQueryBot.Agents;
using SimpleAIQueryBot.Tools;
using Stateless;

public class Chatbot
{
    public enum State
    {
        None,
        Routing,
        TransformQuery,
        LookupData,
        GenerateReport,
    }

    enum Trigger
    {
        UserInput,
        ContextNeeded,
        NoCotextNeeded,
        QueryTransformed,
        DataReady
    }

    public string Invoke(string input, Action<State, QueryContext> onStateChange)
    {
        var context = new QueryContext()
        {
            Query = input,
        };


        var stateMachine = new StateMachine<State, Trigger>(State.None);        

        stateMachine.Configure(State.None)                    
                    .Permit(Trigger.UserInput, State.Routing);                    

        stateMachine.Configure(State.Routing)
                    .OnEntry(() => 
                    { 
                        onStateChange?.Invoke(State.Routing, context); 
                        RouteUserQuery(stateMachine, context);                        
                    })
                    .Permit(Trigger.ContextNeeded, State.TransformQuery)
                    .Permit(Trigger.NoCotextNeeded, State.GenerateReport);


        stateMachine.Configure(State.TransformQuery)
                    .OnEntry(() =>
                    {
                        onStateChange?.Invoke(State.TransformQuery, context);
                        TransformUserQuery(stateMachine, context);
                    })
                    .Permit(Trigger.QueryTransformed, State.LookupData);

        stateMachine.Configure(State.LookupData)
                    .OnEntry(() =>
                    {
                        onStateChange?.Invoke(State.LookupData, context);
                        LookupData(stateMachine, context);
                    })
                    .Permit(Trigger.DataReady, State.GenerateReport);

        stateMachine.Configure(State.GenerateReport)
                    .OnEntry(() =>
                    {
                        onStateChange?.Invoke(State.GenerateReport, context);
                        GenerateReport(stateMachine, context);
                    });

        stateMachine.Fire(Trigger.UserInput);

        return context.Report;
    }

    private void RouteUserQuery(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        Routing routeAgent = new Routing();
        var routingState = routeAgent.Invoke(context.Query);
        if (routingState == RoutingState.ContextRequired)
        {         
            stateMachine.Fire(Trigger.ContextNeeded);
        }
        else
        {            
            stateMachine.Fire(Trigger.NoCotextNeeded);
        }
    }

    private void TransformUserQuery(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        QueryTransformation queryTransformAgent = new QueryTransformation();
        var webSearchQuery = queryTransformAgent.Invoke(context.Query);
        context.TransformedQuery = webSearchQuery;        
        stateMachine.Fire(Trigger.QueryTransformed);
    }

    private void LookupData(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        BingSearch bingSearch = new BingSearch();
        context.Data = bingSearch.SearchAsync(context.TransformedQuery).Result;        
        stateMachine.Fire(Trigger.DataReady);
    }

    private void GenerateReport(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        GenerateReport reportAgent = new GenerateReport();
        context.Report = reportAgent.Invoke(context);
    }
}