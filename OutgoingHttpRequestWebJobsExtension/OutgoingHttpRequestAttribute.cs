using System;

namespace OutgoingHttpRequestWebJobsExtension
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class OutgoingHttpRequestAttribute : Attribute
    {
        public OutgoingHttpRequestAttribute(string uri)
        {
            Uri = new Uri(uri);
        }

        public Uri Uri { get; private set; }
    }
}
