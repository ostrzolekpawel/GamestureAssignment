using System;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Configs;
using GamestureAssignment.UIs;
using Osiris.Configs;
using OsirisGames.EventBroker;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment
{
    // to small project to use DI containers, that's why Installer
    public class Installer : MonoBehaviour
    {
        [SerializeField] private ConfigScriptable<CollectableInfo, CollectableViewData> _viewConfig;
        [SerializeField] private ConfigScriptable<int, Collectable> _dailyRewardsConfig;
        [SerializeField] private DailyRewardsCalendar _calendar;
        [SerializeField] private Button _cheatTime;

        private IViewDataProviderFactory<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>> _viewDataProviderFactory;
        private DailyRewards _dailyRewards;
        private IEventBus _signalBus;

        private void Awake()
        {
            _signalBus = new EventBus();
            _viewDataProviderFactory = new CollectableViewDataProviderFactory(_viewConfig);

            var viewDataProvider = _viewDataProviderFactory.GetViewProvider(CollectableViewType.DailyReward);
            var collectorProvider = new DefaultCollectableCollector(new DefaultInventory());
            var dailyRewardProvider = new ConfigDailyRewardsProvider(_dailyRewardsConfig, 6);
            _dailyRewards = new DailyRewards(viewDataProvider, collectorProvider, dailyRewardProvider, _signalBus);

            _cheatTime.onClick.AddListener(Cheat);
        }

        private void Cheat()
        {
            _signalBus.Fire(new TimeCheatSignal());
        }

        private void Start()
        {
            _dailyRewards.Setup(_calendar);
        }

        private void Update()
        {
            _dailyRewards.Tick();
        }

        private void OnDestroy()
        {
            _dailyRewards?.Dispose();
            _cheatTime.onClick.RemoveListener(Cheat);
        }
    }
}