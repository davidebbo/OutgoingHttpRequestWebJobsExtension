using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace OutgoingHttpRequestWebJobsExtension
{
    // This should be added to BindingFactory
    public static class BindingFactoryExtensions
    {
        public static IBindingProvider BindToAsyncCollector<TAttribute, TMessage>(this BindingFactory factory, Func<TAttribute, TMessage, Task> processMessage) where TAttribute : Attribute
        {
            return factory.BindToAsyncCollector<TAttribute, TMessage>(attrib => new AsyncCollector<TAttribute, TMessage>(attrib, processMessage));
        }

        class AsyncCollector<TAttribute, TMessage> : IAsyncCollector<TMessage>
        {
            private TAttribute _attrib;
            private Func<TAttribute, TMessage, Task> _processMessage;

            public AsyncCollector(TAttribute attrib, Func<TAttribute, TMessage, Task> processMessage)
            {
                _attrib = attrib;
                _processMessage = processMessage;
            }

            public Task AddAsync(TMessage item, CancellationToken cancellationToken = default(CancellationToken))
            {
                return _processMessage(_attrib, item);
            }

            public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult(0);
            }
        }
    }
}
