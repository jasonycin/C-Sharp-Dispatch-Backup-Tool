using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using static Dispatch_Backup_CLI.Ui;
using static Dispatch_Backup_CLI.Dispatch;

namespace Dispatch_Backup_CLI
{
    static class Program
    {
        static void Main()
        {
            // Name & Version (ui.cs)
            Intro();
            /* Prompts for nation name & user-agent. Ratelimit to be included in a later version. Default: 650 ms. (ui.cs)
             * Info => info.nationName, info.uAgent, & info.ratelimit
             */
            var info = PromptData();
            // Get Dispatch List
            // Initial request to get raw data (web.cs)
            (List<string> IDs, JObject json) dispatchList = GetDispatchList(
                $"https://www.nationstates.net/cgi-bin/api.cgi?nation={info.nationName}&q=dispatchlist", 
                info.uAgent);
            Console.WriteLine($"\n{dispatchList.IDs.Count} were found. List of Dispatch ID's: \n{string.Join(", ",dispatchList.IDs)}");
            // Get individual dispatches
            List<string> dispatchContent = GetEachDispatch(dispatchList.IDs, info.uAgent);
            List<string> formattedDispatchContent = FormatResult(dispatchContent, dispatchList.json);
            // Save As File
            SaveFile(formattedDispatchContent, dispatchList.json);
        }
    }

}