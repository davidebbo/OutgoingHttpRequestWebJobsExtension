using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
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
                var bindingFactory = context.Config.BindingFactory;

                var bindingProvider = bindingFactory.BindToGenericValueProvider<OutgoingHttpRequestAttribute>((attribute, paramType) =>
                {
                    return Task.FromResult<IValueBinder>(new TextWriterValueBinder(attribute.Uri));
                });

                IExtensionRegistry extensions = context.Config.GetService<IExtensionRegistry>();
                extensions.RegisterBindingRules<OutgoingHttpRequestAttribute>(bindingProvider);
            }

            private class TextWriterValueBinder : IValueBinder
            {
                string _uri;
                MemoryStream _stream = new MemoryStream();
                TextWriter _writer;

                public TextWriterValueBinder(string uri)
                {
                    _uri = uri;
                }

                public Type Type
                {
                    get
                    {
                        return typeof(TextWriter);
                    }
                }

                public object GetValue()
                {
                    _writer = new StreamWriter(_stream);
                    return _writer;
                }

                public async Task SetValueAsync(object value, CancellationToken cancellationToken)
                {
                    using (var client = new HttpClient())
                    {
                        _writer.Flush();
                        _stream.Seek(0, SeekOrigin.Begin);
                        HttpResponseMessage response = await client.PostAsync(_uri, new StreamContent(_stream));
                    }
                }

                public string ToInvokeString()
                {
                    return $"http request to {_uri}";
                }
            }
        }
    }
}

