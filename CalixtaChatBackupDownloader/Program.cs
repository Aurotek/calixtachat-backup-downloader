using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly;

namespace CalixtaChatBackupDownloader
{
    class Program
    {
        static private string client;
        static private string token;
        static private string folder;

        static void Main(string[] args)
        {            
            if (args == null || args.Length < 3)
            {
                Console.WriteLine("Not enough params");
                Console.WriteLine("usage CalixtaChatBackupDownloader client token folder");
                Console.WriteLine("client: The first word in the URL you use to enter calixtachat. If you access to mycompany.calixtachat.com should be 'mycompany'");
                Console.WriteLine("token: API token used to access the calixta API");
                Console.WriteLine("folder: fullpath to the destination folder where the backups will be downloaded");

                return;
            } else {
                client = args[0];
                token = args[1];
                folder = args[2];
            }       

            Boolean r = DownloadBackups("monthly");
            r = DownloadBackups("daily");

        }

        private static Boolean DownloadBackups(string route)
        {
            string url = "https://" + client + ".calixtachat.com/api/v1/backups/" + route + "?api_token=" + token;

            JToken json = GetJsonAsync(url).Result;

            string path = folder + "\\" + route + "\\";
            Directory.CreateDirectory(path);

            WebClient myWebClient = new WebClient();
            foreach (JToken report in json) {
                string filename = path + report["name"].ToString();
                if (!File.Exists(filename)) {
                    myWebClient.DownloadFile(report["url"].ToString(), filename);
                    Console.WriteLine("{0} descargado correctamente", filename);
                }
            }

            return true;
        }

        private static async Task<JToken> GetJsonAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var policy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(300));
                var response = await policy.ExecuteAsync(() => client.GetAsync(url));

                JToken json = JToken.Parse("{}");

                try
                {
                    string content = await response.Content.ReadAsStringAsync();

                    json = JToken.Parse(content);
                }
                catch (WebException ex)
                {
                    int statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                    json["content"] = ((HttpWebResponse)ex.Response).ToString();
                    json["status_code"] = statusCode;
                }

                return json;
            }
        }
    }
}
