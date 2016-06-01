using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;

namespace OutgoingHttpRequestWebJobsExtension
{
    public static class OutgoingHttpRequestJobHostConfigurationExtensions
    {
        public static void UseOutgoingHttpRequests(this JobHostConfiguration config, HttpClient client = null)
        {
            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new OutgoingHttpRequestExtensionConfig(client ?? new HttpClient()));
        }

        private class OutgoingHttpRequestExtensionConfig : IExtensionConfigProvider
        {
            private readonly HttpClient _client;

            public OutgoingHttpRequestExtensionConfig(HttpClient client)
            {
                _client = client;
            }

            public void Initialize(ExtensionConfigContext context)
            {
                var bindingFactory = context.Config.BindingFactory;

                var converterManager = bindingFactory.ConverterManager;
                converterManager.AddConverter<string, HttpRequestMessage, OutgoingHttpRequestAttribute>(
                    (body, attribute) => BuildHttpRequestMessage(attribute, new StringContent(body)));

                converterManager.AddConverter<HttpContent, HttpRequestMessage, OutgoingHttpRequestAttribute>(
                    (content, attribute) => BuildHttpRequestMessage(attribute, content));

                var bindingOut = bindingFactory.BindToAsyncCollector<OutgoingHttpRequestAttribute, HttpRequestMessage>(attrib => new HttpSender { _client = this._client, _attrib = attrib });

                var bindingClient = bindingFactory.BindToExactType<OutgoingHttpRequestAttribute, HttpClient>(attrib => _client);

                var bindingProvider = bindingFactory.BindToGenericValueProvider<OutgoingHttpRequestAttribute>((attrib, paramType) =>
                {
                    return Task.FromResult<IValueBinder>(new TextWriterValueBinder(attrib.Uri));
                });

                context.RegisterBindingRules<OutgoingHttpRequestAttribute>(bindingOut, bindingClient, bindingProvider);
            }

            static HttpRequestMessage BuildHttpRequestMessage(OutgoingHttpRequestAttribute attribute, HttpContent content)
            {
                return new HttpRequestMessage(HttpMethod.Post, attribute.Uri)
                {
                    Content = content
                };
            }

            class HttpSender : IAsyncCollector<HttpRequestMessage>
            {
                public OutgoingHttpRequestAttribute _attrib;
                public HttpClient _client;

                public async Task AddAsync(HttpRequestMessage item, CancellationToken cancellationToken = default(CancellationToken))
                {
                    if (item.RequestUri == null) item.RequestUri = new Uri(_attrib.Uri);

                    item.Method = new HttpMethod("POST");
                    await _client.SendAsync(item);
                }

                public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
                {
                    return Task.FromResult(0);
                }
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

