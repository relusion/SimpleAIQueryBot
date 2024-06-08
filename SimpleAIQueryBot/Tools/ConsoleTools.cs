using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAIQueryBot.Tools
{
    public class ConsoleTools
    {
        public static void PrintIndentedAndColored(string text, int indentLevel, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            // Set the console text color
            Console.ForegroundColor = foregroundColor;

            // Set the console background color
            Console.BackgroundColor = backgroundColor;

            // Create the indentation string
            string indent = new string(' ', indentLevel);

            // Print the indented and colored text
            Console.WriteLine($"{indent}{text}");

            Console.ResetColor();

        }
    }
}