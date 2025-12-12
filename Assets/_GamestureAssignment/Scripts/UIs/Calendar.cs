using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Collectables;
using GamestureAssignment.Configs;
using OsirisGames.EventBroker;

namespace GamestureAssignment.UIs
{
    public interface ICalendar : IDisposable
    {

    }
    public class Calendar : IDisposable // todo also create interface for daily rewards
    {
        private readonly IDailyRewardsProvider<Collectable> _dailyRewardsProvider;
        private readonly IEventBus _signalBus;
        private readonly IViewDataProvider<CollectableInfo, CollectableViewData> _viewDataProvider;
        private int _daysInCalendar = 6;
        private int _currentCollectedDay;
        private int _nextRewardDuration = 3600 * 24; // take those information from config?
        private DateTime _nextRewardAvailable;
        private CalendarView _calendar;
        private IReadOnlyList<Collectable> _currentDailyRewards;

        private readonly ICollectorProvider<Collectable, CollectableCollectorArgs> _collector;

        public Calendar(
                    IViewDataProvider<CollectableInfo, CollectableViewData> viewDataProvider,
                    ICollectorProvider<Collectable, CollectableCollectorArgs> collector,
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

        public async void Setup(CalendarView calendar)
        {
            _calendar = calendar;

            await CreateViews();

            _calendar.Collected += Collect;

            _nextRewardAvailable = DateTime.UtcNow.AddSeconds(_nextRewardDuration); // ideal take it from somewhere
        }

        private async UniTask CreateViews()
        {
            _currentDailyRewards = await _dailyRewardsProvider.GetDailyRewards();
            _daysInCalendar = _currentDailyRewards.Count;

            for (int i = 0; i < _currentDailyRewards.Count; i++)
            {
                var reward = _currentDailyRewards[i];
                var view = _viewDataProvider.GetViewData(reward.Info);
                _calendar.SetupViews(i, view, reward);
            }
        }

        public async void Collect()
        {
            var reward = _currentDailyRewards[_currentCollectedDay];//.GetData(_currentCollectedDay);
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
            var now = DateTime.UtcNow;
            var isAvailable = now > _nextRewardAvailable;
            var leftTime = now - _nextRewardAvailable;
            _calendar.UpdateInfo(leftTime, isAvailable);
        }

        public void Dispose()
        {
            _calendar.Collected -= Collect;
            _signalBus.Unsubscribe<TimeCheatSignal>(IncreaseTimer);
        }
    }
}