using System;
using System.IO;
using System.Net;

namespace Dispatch_Backup_CLI
{
    public class Web
    {
        public static string ApiRequest(string url, string uAgent)
        {
            Console.WriteLine($"Sending HTTP request to: {url}");
            // Setup HTTP Request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("User-Agent", uAgent);
            request.Method = "GET";
            // Send Request
            var reqAnswer = request.GetResponse();
            var reqStream = reqAnswer.GetResponseStream();
            // Read Response/Answer
            var responseReader = new StreamReader(reqStream!);
            var response = responseReader.ReadToEnd();
            // Cleanup
            responseReader.Close();
            return response;
        }
    }
}