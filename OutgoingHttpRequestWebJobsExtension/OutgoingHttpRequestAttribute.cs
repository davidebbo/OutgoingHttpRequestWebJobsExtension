using System;
using Microsoft.Azure.WebJobs;

namespace OutgoingHttpRequestWebJobsExtension
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class OutgoingHttpRequestAttribute : Attribute
    {
        public OutgoingHttpRequestAttribute(string uri)
        {
            Uri = uri;
        }

        [AutoResolve]
        public string Uri { get; private set; }
    }
}
