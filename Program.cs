using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string token, path, groupid;
            int count = 1;
            if (args.Length < 1)
            {
                Console.Write("Enter token: ");
                token = Console.ReadLine();
            }
            else { token = args[0]; }
            if (args.Length < 2)
            {
                Console.Write("Enter file path: ");
                path = Console.ReadLine();
                path = path.Trim('\"');
            }
            else { path = args[1]; }
            if (args.Length < 3) { Console.Write("Enter group id: "); groupid  = Console.ReadLine(); }
            else { groupid= args[2]; }
            if (args.Length < 4) { Console.Write("Enter uploads count: "); count = int.Parse(Console.ReadLine()); }
            else { count = int.Parse(args[3]); }
            for (int i = 0; i < count; i++)
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.106 Safari/537.36");
                webClient.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                webClient.QueryString.Add("v", "5.107");
                if(groupid.Length>2)webClient.QueryString.Add("group_id", groupid);
                webClient.QueryString.Add("access_token", token);
                webClient.QueryString.Add("file_size", new FileInfo(path).Length.ToString());
                string result = webClient.DownloadString("https://api.vk.com/method/shortVideo.create");
                if (result.Contains("Flood control")) {
                    Console.WriteLine("Flood control. Exiting...");
                    break;
                }
                Console.WriteLine("Response: " + result);
                JObject jObject = JObject.Parse(result);
                string uploadURL = jObject.SelectToken("$.response.upload_url").Value<string>();
                Console.WriteLine("Upload URL: " + uploadURL);
                WebClient uploader = new WebClient();
                Console.Write("Uploading clip... ");
                uploader.UploadFile(uploadURL, path);
                Console.WriteLine("Uploaded!");
            }
        }
    }
}
