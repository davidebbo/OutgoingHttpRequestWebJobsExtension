using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;

namespace OutgoingHttpRequestWebJobsExtension
{
    public static class OutgoingHttpRequestJobHostConfigurationExtensions
    {
        public static void UseOutgoingHttpRequests(this JobHostConfiguration config)
        {
            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new OutgoingHttpRequestExtensionConfig());
        }

        private class OutgoingHttpRequestExtensionConfig : IExtensionConfigProvider
        {
            public void Initialize(ExtensionConfigContext context)
            {
                // Register our extension binding providers
                context.Config.RegisterBindingExtensions(new OutgoingHttpRequestAttributeBindingProvider());
            }
        }
    }
}
