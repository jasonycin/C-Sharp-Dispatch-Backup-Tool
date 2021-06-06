using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Dispatch_Backup_CLI
{
    public class Web
    {
        public static string ApiRequest(string url, string uAgent)
        {
            Console.WriteLine($"Sending HTTP request to: {url}");
            try
            {
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
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nERROR: ");
                Console.Write("The nation you entered likely does not exist. Shutting down in 5 seconds...");
                Thread.Sleep(5000);
                throw;
            }
        }
    }
}