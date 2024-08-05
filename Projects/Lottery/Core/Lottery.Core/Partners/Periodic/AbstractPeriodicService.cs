using HnMicro.Framework.Options;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Lottery.Core.Partners.Periodic
{
    public abstract class AbstractPeriodicService<T> : IPeriodicService
    {
        protected ILogger<T> Logger;
        protected IServiceProvider ServiceProvider;
        protected ServiceOption ServiceOption;
        private volatile bool _initial;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;
        private readonly ConcurrentQueue<IBaseMessageModel> _queue = new();

        protected AbstractPeriodicService(ILogger<T> logger, IServiceProvider serviceProvider, ServiceOption serviceOption)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            ServiceOption = serviceOption;

            Init();
        }

        protected abstract PartnerType Partner { get; set; }

        private void Init()
        {
            if (_initial) return;

            try
            {
                _cts = new CancellationTokenSource();
                _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            }
            finally
            {
                _initial = true;
            }
        }

        public async Task Start()
        {
            Logger.LogInformation($"{ServiceOption.Name} - Started.");

            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                var messages = ReadQueue();
                if (messages.Count == 0) continue;

                await InternalProcessMessages(messages);
            }
        }

        protected abstract Task InternalProcessMessages(List<IBaseMessageModel> messages);

        private List<IBaseMessageModel> ReadQueue()
        {
            var messages = new List<IBaseMessageModel>();
            while (_queue.TryDequeue(out var message))
            {
                if (message.Partner != Partner) continue;
                messages.Add(message);
            }
            return messages;
        }

        public void Enqueue(IBaseMessageModel message)
        {
            _queue.Enqueue(message);
        }

        public void Enqueue(string message)
        {
            var itemMessage = JsonConvert.DeserializeObject<IBaseMessageModel>(message, CommonHelper.CreateJsonSerializerSettings());
            if (itemMessage == null) return;
            _queue.Enqueue(itemMessage);
        }
    }
}
