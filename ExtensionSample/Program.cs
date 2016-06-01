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

            //string methodName = "StringTest";
            //string methodName = "TextWriterTest";
            //string methodName = "HttpRequestMessageTest";
            //string methodName = "HttpRequestMessageCollectorTest";
            //string methodName = "HttpContentCollectorTest";
            //string methodName = "StringCollectorTest";
            string methodName = "POCOTest";

            var method = typeof(Program).GetMethod(methodName);
            host.Call(method, new Dictionary<string, object>
            {
                {"uri", uri },
                {"input", inputString }
            });
        }

        public static void TextWriterTest(
            string input,
            [OutgoingHttpRequest("{uri}")] TextWriter writer,
            TextWriter logger)
        {
            logger.Write(input);
            writer.Write("TextWriterTest: ");
            writer.Write(input);
        }

        // Uses string-->HttpRequestMessage converter.
        // Works nicely cross language
        public static void StringTest(
            string input,
            [OutgoingHttpRequest("{uri}")] out string output,
            TextWriter logger)
        {
            logger.Write(input);
            output = input.ToUpper();
        }

        public static void HttpRequestMessageTest(
            string input,
            [OutgoingHttpRequest("{uri}")] out HttpRequestMessage output,
            TextWriter logger)
        {
            output = new HttpRequestMessage { Content = new StringContent(input) };
        }

        public static void HttpRequestMessageCollectorTest(
            string input,
            [OutgoingHttpRequest("{uri}")] ICollector<HttpRequestMessage> output,
            TextWriter logger)
        {
            // Send two request with different casing
            output.Add(new HttpRequestMessage { Content = new StringContent(input.ToLower()) });
            output.Add(new HttpRequestMessage { Content = new StringContent(input.ToUpper()) });
        }

        public static void HttpContentCollectorTest(
            string input,
            [OutgoingHttpRequest("{uri}")] ICollector<HttpContent> output,
            TextWriter logger)
        {
            // Send two request with different casing
            output.Add(new StringContent(input.ToLower()));
            output.Add(new StringContent(input.ToUpper()));
        }

        public static void StringCollectorTest(
            string input,
            [OutgoingHttpRequest("{uri}")] ICollector<string> output,
            TextWriter logger)
        {
            output.Add(input.ToLower());
            output.Add(input.ToUpper());
        }

        // Poco json serialization example
        // Also cross language.
        public class Body
        {
            public string key1 { get; set; }
            public string key2 { get; set; }
        }

        public static void POCOTest(
            string input,
            [OutgoingHttpRequest("{uri}")] out Body output,
            TextWriter logger)
        {
            output = new Body
            {
                key1 = "value1",
                key2 = "value2"
            };
        }

        // full control binding for "power" usage.
        // C# only.
        //public static async Task HttpClientTest(HttpClient client)
        //{
        //    var request = new HttpRequestMessage();
        //    await client.SendAsync(request);
        //}
    }
}