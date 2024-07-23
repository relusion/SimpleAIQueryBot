using dotenv.net;
using SimpleAIQueryBot.Agents;
using SimpleAIQueryBot.Tools;
using static Chatbot;

// make sure you added CopyToOutputDirectory for .env file in your cproj file
DotEnv.Load();

ConsoleTools.PrintIndentedAndColored("Welcome to the Chatbot! Type 'exit' to end the conversation.", 0, ConsoleColor.Yellow, ConsoleColor.Black);
var bot = new Chatbot();

void OnBotStateChange(State newState, QueryContext newContext)
{
     if(newState == State.LookupData)
    {
        // user query transformed to a data lookup
        ConsoleTools.PrintIndentedAndColored($"TransformedQuery: {newContext.TransformedQuery}", 6, ConsoleColor.Yellow, ConsoleColor.Black);        
    }

    ConsoleTools.PrintIndentedAndColored($"Switched To State: {newState}", 5, ConsoleColor.Green, ConsoleColor.Black);
}

while (true)
{
    Console.Write("You: ");
    string userInput = Console.ReadLine();

    if (userInput.ToLower() == "exit")
    {
        Console.WriteLine("Chatbot: Goodbye!f");
        break;
    }

    string botResponse = bot.Invoke(userInput, OnBotStateChange);
    Console.WriteLine($"Chatbot: {botResponse}");
}