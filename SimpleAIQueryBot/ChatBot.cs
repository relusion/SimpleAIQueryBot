using System.Diagnostics;
using Agents;
using SimpleAIQueryBot.Agents;
using SimpleAIQueryBot.Tools;
using Stateless;

public class Chatbot
{
    enum State
    {
        Routing,
        TransformQuery,
        LookupData,
        GenerateReport,
    }

    enum Trigger
    {
        ContextNeeded,
        NoCotextNeeded,
        QueryTransformed,
        DataReady
    }

    public Chatbot()
    {

    }

    public string Invoke(string input)
    {
        var context = new QueryContext()
        {
            Query = input,
        };


        var stateMachine = new StateMachine<State, Trigger>(State.Routing);

        stateMachine.Configure(State.Routing)
                    .Permit(Trigger.ContextNeeded, State.TransformQuery)
                    .Permit(Trigger.NoCotextNeeded, State.GenerateReport);


        stateMachine.Configure(State.TransformQuery)
                    .OnEntry(() => TransformUserQuery(stateMachine, context))
                    .Permit(Trigger.QueryTransformed, State.LookupData);

        stateMachine.Configure(State.LookupData)
                    .OnEntry(() => { LookupData(stateMachine, context); })
                    .Permit(Trigger.DataReady, State.GenerateReport);

        stateMachine.Configure(State.GenerateReport)
        .OnEntry(() => GenerateReport(stateMachine, context));

        RouteUserQuery(stateMachine, context);

        return context.Report;
    }

    private void RouteUserQuery(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        Routing routeAgent = new Routing();
        var routingState = routeAgent.Invoke(context.Query);
        if (routingState == RoutingState.ContextRequired)
        {
            Debug.WriteLine("ContextNeeded");
            stateMachine.Fire(Trigger.ContextNeeded);
        }
        else
        {
            Debug.WriteLine("NoCotextNeeded");
            stateMachine.Fire(Trigger.NoCotextNeeded);
        }
    }

    private void TransformUserQuery(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        QueryTransformation queryTransformAgent = new QueryTransformation();
        var webSearchQuery = queryTransformAgent.Invoke(context.Query);
        context.TransformedQuery = webSearchQuery;
        Debug.WriteLine("QueryTransformed");
        stateMachine.Fire(Trigger.QueryTransformed);
    }

    private void LookupData(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        BingSearch bingSearch = new BingSearch();
        context.Data = bingSearch.SearchAsync(context.TransformedQuery).Result;
        Debug.WriteLine("DataReady");
        stateMachine.Fire(Trigger.DataReady);
    }

    private void GenerateReport(StateMachine<State, Trigger> stateMachine, QueryContext context)
    {
        GenerateReport reportAgent = new GenerateReport();
        context.Report = reportAgent.Invoke(context);
    }
}