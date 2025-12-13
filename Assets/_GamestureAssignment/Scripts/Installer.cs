using System.Threading.Tasks;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Collectables;
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
        [SerializeField] private FixedDailyRewardPolicy _dailyRewadTimePolicy;
        [SerializeField] private CalendarView _calendar;
        [SerializeField] private HudView _hudView;
        [SerializeField] private Button _cheatTime;

        private IViewDataProviderFactory<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>> _viewDataProviderFactory;
        private Calendar _callendar;
        private IEventBus _signalBus;
        private IHudMediator _hudMediator;

        private void Awake()
        {
            _signalBus = new EventBus();
            _viewDataProviderFactory = new CollectableViewDataProviderFactory(_viewConfig, _hudConfig);

            var viewDataProviderDaily = _viewDataProviderFactory.GetViewProvider(CollectableViewType.DailyReward);
            var viewDataProviderHud = _viewDataProviderFactory.GetViewProvider(CollectableViewType.HUD);
            var collectorProvider = new InventoryCollectableCollector(new DefaultInventory(), _signalBus);
            var dailyRewardProvider = new DailyRewardsConfigProvider(_dailyRewardsConfig);
            var timerProvider = new SystemTimeProvider();
            _callendar = new Calendar(viewDataProviderDaily, collectorProvider, dailyRewardProvider, _dailyRewadTimePolicy, timerProvider, _signalBus);

            _hudMediator = new HudMediator(_hudView, _collectableCatalog.AllCollectables, viewDataProviderHud, _signalBus);

            _cheatTime.onClick.AddListener(Cheat);
        }

        private async void Start()
        {
            await _callendar.SetupAsync(_calendar);
            _hudMediator.Setup();
        }

        private void Cheat()
        {
            _signalBus.Fire(new TimeCheatSignal());
        }

        private void Update()
        {
            _callendar.Tick();
        }

        private void OnDestroy()
        {
            _callendar?.Dispose();
            _cheatTime.onClick.RemoveListener(Cheat);
            _hudMediator?.Dispose();
        }
    }
}