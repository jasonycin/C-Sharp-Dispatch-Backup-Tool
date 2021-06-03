using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Dispatch_Backup_CLI.web;

namespace Dispatch_Backup_CLI
{
    public class dispatch
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static List<string> GetDispatchList(string url, string uAgent)
        {
            // Send API Request
            var res= ApiRequest(url, uAgent);
            // Load response into XML document
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(res);
            // Convert XML Document to JSON (and then parses it) for easier handling. Uses the json.NET library.
            string jsonRaw = JsonConvert.SerializeXmlNode(xml);
            JObject json = JObject.Parse(jsonRaw);
            // Creates a list, iterates through all dispatch ID's and pushes it to the list, and then returns it.
            List<string> dispatchIDs = new List<string>();
            foreach (var id in json["NATION"]["DISPATCHLIST"]["DISPATCH"])
            {
                dispatchIDs.Add(id["@id"].ToString());
            };
            return dispatchIDs;
        }
    }
}