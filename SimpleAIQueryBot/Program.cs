using dotenv.net;

// make sure you added CopyToOutputDirectory for .env file in your cproj file
DotEnv.Load();

Console.WriteLine("Welcome to the Chatbot! Type 'exit' to end the conversation.");
var bot = new Chatbot();

while (true)
{
    Console.Write("You: ");
    string userInput = Console.ReadLine();

    if (userInput.ToLower() == "exit")
    {
        Console.WriteLine("Chatbot: Goodbye!f");
        break;
    }

    string botResponse = bot.Invoke(userInput);
    Console.WriteLine($"Chatbot: {botResponse}");
}