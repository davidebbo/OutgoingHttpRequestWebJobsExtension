// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Host;
using OutgoingHttpRequestWebJobsExtension;

namespace ExtensionsSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new JobHostConfiguration();

            config.UseDevelopmentSettings();

            config.UseOutgoingHttpRequests();

            var host = new JobHost(config);
            var method = typeof(Program).GetMethod("MyCoolMethod");
            host.Call(method);
        }

        public static void MyCoolMethod(
            [OutgoingHttpRequest(@"http://requestb.in/19xvbmc1")] TextWriter writer)
        {
            writer.Write("Test sring sent to OutgoingHttpRequest!");
        }
    }
}