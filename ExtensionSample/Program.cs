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
            string inputString = args.Length > 0 ? args[0] : "Some test string";

            var config = new JobHostConfiguration();

            config.UseDevelopmentSettings();

            config.UseOutgoingHttpRequests();

            var host = new JobHost(config);
            var method = typeof(Program).GetMethod("MyCoolMethod");
            host.Call(method, new Dictionary<string, object>
            {
                {"input", inputString }
            });
        }

        public static void MyCoolMethod(
            string input,
            [OutgoingHttpRequest(@"http://requestb.in/19xvbmc1")] TextWriter writer,
            TextWriter logger)
        {
            logger.Write(input);
            writer.Write(input);
        }
    }
}