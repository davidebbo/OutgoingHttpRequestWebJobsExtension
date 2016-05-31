// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using OutgoingHttpRequestWebJobsExtension;

namespace ExtensionsSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                System.Console.WriteLine("Pass the uri and test string on command line");
                return;
            }

            string uri = args[0];
            string inputString = args[1];

            var config = new JobHostConfiguration();

            config.UseDevelopmentSettings();

            config.UseOutgoingHttpRequests();

            var host = new JobHost(config);

            string methodName = "SendFromString";
            //string methodName = "MyCoolMethod";

            var method = typeof(Program).GetMethod(methodName);
            host.Call(method, new Dictionary<string, object>
            {
                {"uri", uri },
                {"input", inputString }
            });
        }

        public static void MyCoolMethod(
            string input,
            [OutgoingHttpRequest("{uri}")] TextWriter writer,
            TextWriter logger)
        {
            logger.Write(input);
            writer.Write("MyCoolMethod: ");
            writer.Write(input);
        }

        // Uses string-->HttpRequestMessage converter.
        // Works nicely cross language
        public static void SendFromString(
            string input,
            [OutgoingHttpRequest("{uri}")] out string output,
            TextWriter logger)
        {
            logger.Write(input);
            output = input.ToUpper();
        }

        public static void Send(out HttpRequestMessage output)
        {
            output = new HttpRequestMessage { };
        }

        public static void Send(ICollector<HttpRequestMessage> output)
        {

        }

        public static void Send(ICollector<HttpContent> output)
        {

        }

        public static void Send(ICollector<string> output)
        {
            output.Add("key1=value1&key2=value2");
        }

        // Poco json serialization example
        // Also cross language.
        public class Body
        {
            public string key1 { get; set; }
            public string key2 { get; set; }
        }

        public static void Send(out Body output)
        {
            output = new Body
            {
                key1 = "value1",
                key2 = "value2"
            };
        }

        // full control binding for "power" usage.
        // C# only.
        public static async Task FullControl(HttpClient client)
        {
            var request = new HttpRequestMessage();
            await client.SendAsync(request);
        }
    }
}