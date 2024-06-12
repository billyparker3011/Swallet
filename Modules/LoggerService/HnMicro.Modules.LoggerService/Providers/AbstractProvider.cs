using HnMicro.Framework.Logger.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace HnMicro.Modules.LoggerService.Providers
{
    public abstract class AbstractProvider : IProvider
    {
        private volatile bool _disposed;
        private volatile bool _stop;
        protected IServiceProvider ServiceProvider;
        protected IConfiguration Configuration;
        private readonly ConcurrentQueue<LogModel> _queue = new();

        public AbstractProvider(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            Init();
        }

        private void Init()
        {
            new Thread(Loop) { IsBackground = true }.Start();
        }

        private void Loop(object obj)
        {
            try
            {
                var d = new List<LogModel>();
                while (!_stop)
                {
                    d.Clear();

                    LogModel message;
                    while (_queue.TryDequeue(out message))
                    {
                        d.Add(message);
                    }

                    InternalProcess(d);

                    Thread.Sleep(50);
                }
            }
            finally
            {
                _disposed = true;
            }
        }

        protected abstract void InternalProcess(List<LogModel> messages);

        public void Dispose()
        {
            _stop = true;

            for (var i = 0; i < 10; i++)
            {
                if (!_disposed) Thread.Sleep(100);
            }
        }

        public void Enqueue(LogModel message)
        {
            _queue.Enqueue(message);
        }
    }
}
