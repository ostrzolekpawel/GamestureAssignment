using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamestureAssignment.Configs;
using Osiris.Configs;

namespace GamestureAssignment.CollectableDisplayer
{
    public interface IViewDataProvider<in TData, TViewData>
    {
        TViewData GetViewData(TData data);
        UniTask<TViewData> GetViewDataAsync(TData data);
    }

    public class CollectableViewDataProvider : IViewDataProvider<CollectableInfo, CollectableViewData>
    {
        private readonly IConfig<CollectableInfo, CollectableViewData> _config;

        public CollectableViewDataProvider(IConfig<CollectableInfo, CollectableViewData> config)
        {
            _config = config;
        }

        public CollectableViewData GetViewData(CollectableInfo data)
        {
            return _config.GetData(data);
        }

        public UniTask<CollectableViewData> GetViewDataAsync(CollectableInfo data)
        {
            return UniTask.FromResult(_config.GetData(data));
        }
    }

    public enum CollectableViewType // TODO use it to load different views?
    {
        DailyReward,
        HUD
    }

    public interface IViewDataProviderFactory<in TType, out IViewDataProvider>
    {
        IViewDataProvider GetViewProvider(TType type);
    }

    public class CollectableViewDataProviderFactory : IViewDataProviderFactory<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>>
    {
        private readonly Dictionary<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>> _map;

        public CollectableViewDataProviderFactory(IConfig<CollectableInfo, CollectableViewData> dailyRewardsConfig, IConfig<CollectableInfo, CollectableViewData> hudConfig)
        {
            _map = new Dictionary<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>>
            {
                [CollectableViewType.DailyReward] = new CollectableViewDataProvider(dailyRewardsConfig),
                [CollectableViewType.HUD] = new CollectableViewDataProvider(hudConfig)
            };
        }

        // get different views

        public IViewDataProvider<CollectableInfo, CollectableViewData> GetViewProvider(CollectableViewType type)
        {
            if (_map.TryGetValue(type, out var provider))
            {
                return provider;
            }

            throw new Exception($"No provider for type [{type}]");
        }
    }
}