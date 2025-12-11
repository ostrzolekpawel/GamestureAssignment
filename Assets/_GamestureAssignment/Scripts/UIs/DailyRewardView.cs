using System;
using Cysharp.Threading.Tasks;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Configs;
using Osiris.Configs;
using OsirisGames.EventBroker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class DailyRewardView : MonoBehaviour, ICollectableView
    {
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmount;

        public Image Image => _rewardIcon;
        public TextMeshProUGUI Text => _rewardAmount;

        public void Setup(CollectableViewData viewData, Collectable data)
        {
            _rewardIcon.sprite = viewData.Icon;
            _rewardAmount.text = data.Amount.ToString();

            // reset view here
            UnCollect();
        }

        public void Collect()
        {
            // change view
            Debug.Log("Collected");
            _rewardAmount.text = "Collected";
        }

        private void UnCollect()
        {
            // change view
            Debug.Log("UnCollected");
        }
    }

    public class DailyRewards : IDisposable // todo also create interface for daily rewards
    {
        private readonly IDailyRewardsProvider<Collectable> _dailyRewardsProvider;
        private readonly IEventBus _signalBus;
        private readonly IViewDataProvider<CollectableInfo, CollectableViewData> _viewDataProvider;
        private const int _daysInCalendar = 6;
        private int _currentCollectedDay;
        private int _nextRewardDuration = 3600 * 24; // take those information from config?
        private DateTime _nextRewardAvailable;
        private DailyRewardsCalendar _calendar;
        private IConfig<int, Collectable> _currentDailyRewards;

        private readonly ICollector<Collectable, CollectableCollectorArgs> _collector;

        public DailyRewards(
                    IViewDataProvider<CollectableInfo, CollectableViewData> viewDataProvider,
                    ICollector<Collectable, CollectableCollectorArgs> collector,
                    IDailyRewardsProvider<Collectable> dailyRewarsProvider,
                    IEventBus signalBus)
        {
            _viewDataProvider = viewDataProvider;
            _collector = collector;
            _dailyRewardsProvider = dailyRewarsProvider;
            _signalBus = signalBus;

            _signalBus.Subscribe<TimeCheatSignal>(IncreaseTimer);
        }

        private void IncreaseTimer(TimeCheatSignal signal)
        {
            _nextRewardAvailable = DateTime.UtcNow;
        }

        public async void Setup(DailyRewardsCalendar calendar)
        {
            _calendar = calendar;

            await CreateViews();

            _calendar.Collected += Collect;

            _nextRewardAvailable = DateTime.UtcNow.AddSeconds(_nextRewardDuration); // ideal take it from somewhere
        }

        private async UniTask CreateViews()
        {
            _currentDailyRewards = await _dailyRewardsProvider.GetDailyRewards();
            for (int i = 0; i < _daysInCalendar; i++)
            {
                var reward = _currentDailyRewards.GetData(i);
                var view = _viewDataProvider.GetViewData(reward.Info);
                _calendar.SetupViews(i, view, reward);
            }
        }

        public async void Collect()
        {
            var reward = _currentDailyRewards.GetData(_currentCollectedDay);
            var view = _calendar.GetView(_currentCollectedDay);
            await _collector.CollectAsync(reward, default, new CollectableCollectorArgs // do cts later
            {
                View = view
            });

            _currentCollectedDay = (_currentCollectedDay + 1) % _daysInCalendar;
            _nextRewardAvailable = DateTime.UtcNow.AddSeconds(_nextRewardDuration); // duplication

            view.Collect();

            if (_currentCollectedDay == 0)
            {
                await CreateViews();
            }
        }

        public void Tick()
        {
            var isAvailable = DateTime.UtcNow > _nextRewardAvailable;
            var leftTime = DateTime.UtcNow - _nextRewardAvailable;
            _calendar.UpdateInfo(leftTime, isAvailable);
        }

        public void Dispose()
        {
            _calendar.Collected -= Collect;
            _signalBus.Unsubscribe<TimeCheatSignal>(IncreaseTimer);
        }
    }

    public class TimeCheatSignal
    {
    }
}