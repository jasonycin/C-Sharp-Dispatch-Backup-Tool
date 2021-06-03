using System;

namespace Dispatch_Backup_CLI
{
    public class ui
    {
        public static void Intro()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Dispatch Backup Tool");
            Console.ResetColor();
            Console.Write(", v1.0 (C#)");
        }

        public static (string nationName, int ratelimit, string uAgent) PromptData()
        {
            Console.Write("\n----------\n" +
                          "Enter the nation name to backup from (without prefix): ");
            string nationName = Console.ReadLine();
            Console.Write("\nEnter the User-Agent. This should be YOUR nation name or email address: ");
            string uAgent = Console.ReadLine();
            // Future release will support setting a custom ratelimit.
            int ratelimit = 650;
            return (nationName, ratelimit, uAgent);
        }
    }
}