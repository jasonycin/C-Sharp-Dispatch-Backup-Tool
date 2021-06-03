using System;
using System.Collections.Generic;
using static Dispatch_Backup_CLI.ui;
using static Dispatch_Backup_CLI.dispatch;

namespace Dispatch_Backup_CLI
{
    static class Program
    {
        static void Main()
        {
            // Name & Version (ui.cs)
            Intro();
            // Prompts for nation name & user-agent. Ratelimit to be included in a later version. Default: 650 ms. (ui.cs)
            var info = PromptData();
            // Get Dispatch List
            // Initial request to get raw data (web.cs)
            List<string> dispatchList = GetDispatchList($"https://www.nationstates.net/cgi-bin/api.cgi?nation={info.nationName}&q=dispatchlist", 
                info.uAgent);
            Console.WriteLine("\nList of ID's: {0}",string.Join("",dispatchList));
            // Prevents auto-termination of Console Application
            Console.ReadKey();
        }
    }

}