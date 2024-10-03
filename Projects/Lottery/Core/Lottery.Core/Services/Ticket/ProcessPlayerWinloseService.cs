using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Services;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Services.Caching.Winlose;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Lottery.Core.Services.Ticket;

public class ProcessPlayerWinloseService : HnMicroBaseService<ProcessPlayerWinloseService>, IProcessPlayerWinloseService
{
    private readonly CancellationTokenSource _cts = new();
    private PeriodicTimer _timer;
    private readonly ConcurrentQueue<ProcessPlayerWinloseModel> _queue = new();
    private readonly IProcessWinloseService _processWinloseService;

    public ProcessPlayerWinloseService(ILogger<ProcessPlayerWinloseService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService,
        IProcessWinloseService processWinloseService) : base(logger, serviceProvider, configuration, clockService)
    {
        Init();
        _processWinloseService = processWinloseService;
    }

    public void Enqueue(SportKind sportKind, Dictionary<long, Dictionary<string, decimal>> playerProcessed)
    {
        foreach (var item in playerProcessed)
        {
            foreach (var winloseItem in item.Value)
            {
                _queue.Enqueue(new ProcessPlayerWinloseModel
                {
                    SportKindId = sportKind.ToInt(),
                    PlayerId = item.Key,
                    PartOfWinloseMainKey = winloseItem.Key,
                    WinloseValue = winloseItem.Value
                });
            }
        }
    }

    public async Task Start()
    {
        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            var items = ReadQueue();
            if (items.Count == 0) continue;

            await InternalProcess(items);
        }
    }

    private async Task InternalProcess(Dictionary<int, Dictionary<long, Dictionary<string, decimal>>> items)
    {
        //  Key = SportTypeId
        //  Value = Dict
        //      Key = PlayerId
        //      Value = Dict
        //          Key = Part of Winlose MainKey
        //          Value = Winlose Value
        var d = new Dictionary<string, Dictionary<string, decimal>>();
        foreach (var sportItem in items)
        {
            foreach (var playerItem in sportItem.Value)
            {
                foreach (var winloseItem in playerItem.Value)
                {
                    var mainKey = sportItem.Key.GetPlayerWinloseMainKey(playerItem.Key, winloseItem.Key);
                    if (!d.TryGetValue(mainKey, out Dictionary<string, decimal> vals))
                    {
                        vals = new Dictionary<string, decimal>();
                        d[mainKey] = vals;
                    }

                    var subKey = playerItem.Key.GetPlayerWinloseSubKey();
                    vals[subKey] = winloseItem.Value;
                }
            }
        }
        await _processWinloseService.UpdateWinloseCache(d);
    }

    private Dictionary<int, Dictionary<long, Dictionary<string, decimal>>> ReadQueue()
    {
        var items = new Dictionary<int, Dictionary<long, Dictionary<string, decimal>>>();
        while (_queue.TryDequeue(out var itemQueue))
        {
            if (!items.TryGetValue(itemQueue.SportKindId, out Dictionary<long, Dictionary<string, decimal>> playerProcessed))
            {
                playerProcessed = new Dictionary<long, Dictionary<string, decimal>>();
                items[itemQueue.SportKindId] = playerProcessed;
            }

            if (!playerProcessed.TryGetValue(itemQueue.PlayerId, out Dictionary<string, decimal> winloseByKickoffTime))
            {
                winloseByKickoffTime = new Dictionary<string, decimal>();
                playerProcessed[itemQueue.PlayerId] = winloseByKickoffTime;
            }
            winloseByKickoffTime[itemQueue.PartOfWinloseMainKey] = itemQueue.WinloseValue;
        }
        return items;
    }

    private void Init()
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
    }
}