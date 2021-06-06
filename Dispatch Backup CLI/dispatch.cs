using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Dispatch_Backup_CLI.Web;
// ReSharper disable PossibleNullReferenceException

namespace Dispatch_Backup_CLI
{
    public static class Dispatch
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static JObject XmlToJson(string rawXml)
        {
            // Load response into XML document
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawXml);
            // Convert XML Document to JSON (and then parses it) for easier handling. Uses the json.NET library.
            string jsonRaw = JsonConvert.SerializeXmlNode(xml);
            JObject json = JObject.Parse(jsonRaw);
            return json;
        }
        
        public static (List<string> dispatchIDs, JObject jsonDispatchList) GetDispatchList(string url, string uAgent)
        {
            // Send API Request
            var res= ApiRequest(url, uAgent);
            // Convert response to json
            JObject jsonDispatchList = XmlToJson(res);
            // Creates a list, iterates through all dispatch ID's and pushes it to the list, and then returns it.
            List<string> dispatchIDs = new List<string>();
            foreach (var id in jsonDispatchList["NATION"]["DISPATCHLIST"]["DISPATCH"])
            {
                dispatchIDs.Add(id["@id"].ToString());
            }

            return (dispatchIDs, jsonDispatchList);
        }
        
        public static List<string> GetEachDispatch(List<string> dispatchIds, string uAgent)
        {
            // Create new list which will contain the text content.
            List<string> dispatchContent = new List<string>();
            for (int i = 0; i < dispatchIds.Count; i++)
            {
                // Status Update
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("\nRequest: ");
                Console.ResetColor();
                Console.Write($"{i+1} / {dispatchIds.Count}\n");
                // API Requests
                var res = ApiRequest(
                    $"https://www.nationstates.net/cgi-bin/api.cgi?q=dispatch;dispatchid={dispatchIds[i]}", uAgent);
                // Rate Limit
                Thread.Sleep(650);
                // Convert to JSON and add to list which is returned.
                JObject jsonDispatch = XmlToJson(res);
                dispatchContent.Add(jsonDispatch["WORLD"]["DISPATCH"]["TEXT"].ToString());
            }
            return dispatchContent;
        }

        public static List<string> FormatResult(List<string> dispatchContent, JObject dispatchJson)
        {
            for (int i = 0; i < dispatchContent.Count; i++)
            {
                // Extract dispatch information to add to output
                string title = dispatchJson["NATION"]["DISPATCHLIST"]["DISPATCH"][i]["TITLE"]
                    .ToString()
                    .TrimStart('{', '\n')
                    .TrimEnd('\n', '}')
                    .Replace("\"#cdata-section\":", "")
                    .Trim();
                string category = dispatchJson["NATION"]["DISPATCHLIST"]["DISPATCH"][i]["CATEGORY"].ToString();
                string subcategory = dispatchJson["NATION"]["DISPATCHLIST"]["DISPATCH"][i]["SUBCATEGORY"].ToString();
                // Cleanup the content (newlines, [cdata], json artifacts, etc.)
                dispatchContent[i] = dispatchContent[i]
                    .TrimStart('{', '\n')
                    .TrimEnd('\n', '}')
                    .Replace("\"#cdata-section\": \"", "")
                    .Replace("\\r\\n", Environment.NewLine)
                    .Trim()
                    .Insert(0, 
                        $"Title: {title}{Environment.NewLine}" + 
                        $"Category: {category}, Subcategory: {subcategory}{Environment.NewLine}" + 
                        "------------\n");

            }
            return dispatchContent;
        }

        public static void SaveFile(List<string> formattedDispatchContent, JObject dispatchJson)
        {
            // Backups Directory
            Directory.CreateDirectory("Backups");
            // Creates Directory For The Current Day
            // Get the current date.
            DateTime thisDay = DateTime.Today;
            string path = $"{Directory.GetCurrentDirectory()}\\Backups\\{thisDay.ToString("d").Replace("/", "-")}";
            Directory.CreateDirectory(path);
            // Save file and output location
            File.WriteAllLines($"{path}\\{dispatchJson["NATION"]["@id"]}.txt", formattedDispatchContent);
            Console.WriteLine($"\nThe file has been saved to: {path}\\{dispatchJson["NATION"]["@id"]}.txt");
            // Open saved backup.
            var process = new Process {StartInfo = new ProcessStartInfo() {UseShellExecute = true, FileName = $"{path}\\{dispatchJson["NATION"]["@id"]}.txt"}};
            process.Start();
            // Outro Text
            Console.WriteLine("\nThanks for using my tool! It will shutdown automatically you close your backup." +
                              "\nCreated by Heaveria :D");
            process.WaitForExit();
        }
    }
}