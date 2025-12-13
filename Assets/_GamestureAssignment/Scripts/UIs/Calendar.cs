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
    public sealed class Calendar : ICalendar
    {
        private readonly IDailyRewardsProvider<Collectable> _rewards;
        private readonly ICollectorProvider<Collectable, CollectableCollectorArgs> _collector;
        private readonly IViewDataProvider<CollectableInfo, CollectableViewData> _viewData;
        private readonly IEventBus _bus;
        private readonly ITimeProvider _time;
        private readonly IDailyRewardTimePolicy _timePolicy;

        private CalendarView _view;
        private IReadOnlyList<Collectable> _currentRewards;
        private int _currentDay;
        private DateTime _nextAvailable;

        public Calendar(
            IViewDataProvider<CollectableInfo, CollectableViewData> viewData,
            ICollectorProvider<Collectable, CollectableCollectorArgs> collector,
            IDailyRewardsProvider<Collectable> rewards,
            IDailyRewardTimePolicy timePolicy,
            ITimeProvider time,
            IEventBus bus)
        {
            _viewData = viewData;
            _collector = collector;
            _rewards = rewards;
            _timePolicy = timePolicy;
            _time = time;
            _bus = bus;

            _bus.Subscribe<TimeCheatSignal>(OnTimeCheat);
        }

        public async UniTask SetupAsync(CalendarView view)
        {
            _view = view;
            _view.Collected += OnCollected;

            await CreateViewsAsync();
            ResetTimer();
        }

        private async UniTask CreateViewsAsync()
        {
            _currentRewards = await _rewards.GetDailyRewards();

            for (int i = 0; i < _currentRewards.Count; i++)
            {
                var reward = _currentRewards[i];
                var viewData = _viewData.GetViewData(reward.Info);
                _view.SetupViews(i, viewData, reward);
            }
        }

        private async void OnCollected()
        {
            if (_time.UtcNow < _nextAvailable)
                return;

            var reward = _currentRewards[_currentDay];
            var view = _view.GetView(_currentDay);

            await _collector.CollectAsync(reward, default,
                new CollectableCollectorArgs { View = view });

            view.Collect();

            _currentDay = (_currentDay + 1) % _currentRewards.Count;
            ResetTimer();

            if (_currentDay == 0)
            {
                _view.Finished();
                await UniTask.Delay(2500);
                await CreateViewsAsync();
            }
        }

        public void Tick()
        {
            var now = _time.UtcNow;
            var available = now >= _nextAvailable;
            var timeLeft = _nextAvailable - now;

            _view.UpdateInfo(timeLeft, available);
        }

        private void ResetTimer()
        {
            _nextAvailable = _time.UtcNow + _timePolicy.GetNextRewardDelay(new CalendarContext(_currentDay, _currentRewards.Count));
        }

        private void OnTimeCheat(TimeCheatSignal _)
        {
            _nextAvailable = _time.UtcNow;
        }

        public void Dispose()
        {
            _view.Collected -= OnCollected;
            _bus.Unsubscribe<TimeCheatSignal>(OnTimeCheat);
        }
    }

}