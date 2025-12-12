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
        [SerializeField] private ConfigScriptable<CollectableInfo, CollectableViewData> _hudConfig;
        [SerializeField] private DailyRewardsConfig _dailyRewardsConfig;
        [SerializeField] private CollectableCatalog _collectableCatalog;
        [SerializeField] private DailyRewardsCalendar _calendar;
        [SerializeField] private HudView _hudView;
        [SerializeField] private Button _cheatTime;

        private IViewDataProviderFactory<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>> _viewDataProviderFactory;
        private DailyRewards _dailyRewards;
        private IEventBus _signalBus;
        private HUDMediator _hudMediator;

        private void Awake()
        {
            _signalBus = new EventBus();
            _viewDataProviderFactory = new CollectableViewDataProviderFactory(_viewConfig, _hudConfig);

            var viewDataProviderDaily = _viewDataProviderFactory.GetViewProvider(CollectableViewType.DailyReward);
            var viewDataProviderHud = _viewDataProviderFactory.GetViewProvider(CollectableViewType.HUD);
            var collectorProvider = new InventoryCollectableCollector(new DefaultInventory(), _signalBus);
            var dailyRewardProvider = new DailyRewardsConfigProvider(_dailyRewardsConfig);
            _dailyRewards = new DailyRewards(viewDataProviderDaily, collectorProvider, dailyRewardProvider, _signalBus);

            _hudMediator = new HUDMediator(_hudView, _collectableCatalog.AllCollectables, viewDataProviderHud, _signalBus);

            _cheatTime.onClick.AddListener(Cheat);
        }

        private void Start()
        {
            _dailyRewards.Setup(_calendar);
            _hudMediator.Setup();
        }

        private void Cheat()
        {
            _signalBus.Fire(new TimeCheatSignal());
        }

        private void Update()
        {
            _dailyRewards.Tick();
        }

        private void OnDestroy()
        {
            _dailyRewards?.Dispose();
            _cheatTime.onClick.RemoveListener(Cheat);
            _hudMediator?.Dispose();
        }
    }
}