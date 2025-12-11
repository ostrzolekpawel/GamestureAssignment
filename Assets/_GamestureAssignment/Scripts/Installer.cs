using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Configs;
using GamestureAssignment.UIs;
using Osiris.Configs;
using UnityEngine;

namespace GamestureAssignment
{
    // to small project to use DI containers, that's why Installer
    public class Installer : MonoBehaviour
    {
        [SerializeField] private ConfigScriptable<CollectableInfo, CollectableViewData> _viewConfig;
        [SerializeField] private ConfigScriptable<int, Collectable> _dailyRewardsConfig;
        [SerializeField] private DailyRewardsCalendar _calendar;

        private IViewDataProviderFactory<CollectableViewType, IViewDataProvider<CollectableInfo, CollectableViewData>> _viewDataProviderFactory;
        private DailyRewards _dailyRewards;

        private void Awake()
        {
            _viewDataProviderFactory = new CollectableViewDataProviderFactory(_viewConfig);

            var viewDataProvider = _viewDataProviderFactory.GetViewProvider(CollectableViewType.DailyReward);
            _dailyRewards = new DailyRewards(viewDataProvider, _dailyRewardsConfig);
        }

        private void Start()
        {
            _dailyRewards.Setup(_calendar);
        }

        private void Update()
        {
            _dailyRewards.Tick();
        }
    }
}