// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
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
            var method = typeof(Program).GetMethod("MyCoolMethod");
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
            writer.Write(input);
        }
    }
}