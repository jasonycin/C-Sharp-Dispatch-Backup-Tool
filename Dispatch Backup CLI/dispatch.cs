using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Dispatch_Backup_CLI.web;

namespace Dispatch_Backup_CLI
{
    public class dispatch
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
        
        public static List<string> GetDispatchList(string url, string uAgent)
        {
            // Send API Request
            var res= ApiRequest(url, uAgent);
            // Convert response to json
            JObject json = XmlToJson(res);
            // Creates a list, iterates through all dispatch ID's and pushes it to the list, and then returns it.
            List<string> dispatchIDs = new List<string>();
            foreach (var id in json["NATION"]["DISPATCHLIST"]["DISPATCH"])
            {
                dispatchIDs.Add(id["@id"].ToString());
            };
            return dispatchIDs;
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

        public static List<string> CleanupContent(List<string> dispatchContent)
        {
            for (int i = 0; i < dispatchContent.Count; i++)
            {
                dispatchContent[i] = dispatchContent[i]
                    .TrimStart('{', '\n')
                    .Replace("\"#cdata-section\": \"", "")
                    .Trim()
                    .TrimEnd('\n', '}');
            }
            return dispatchContent;
        }
    }
}