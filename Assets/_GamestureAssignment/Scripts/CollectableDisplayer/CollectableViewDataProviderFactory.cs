using System;
using System.Collections.Generic;
using GamestureAssignment.Collectables;
using Osiris.Configs;

namespace GamestureAssignment.CollectableDisplayer
{
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