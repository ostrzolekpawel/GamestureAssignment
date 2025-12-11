using System;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Configs;
using Osiris.Configs;
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
        }

        private void UnCollect()
        {
            // change view
        }
    }

    public class DailyRewards
    {
        private IConfig<int, Collectable> _dailyRewardConfig;

        private const int _daysInCalendar = 6;
        private int _currentCollectedDay;
        private int _nextRewardDuration = 3600 * 24;
        private float _timer;
        private DateTime _nextRewardAvailable;
        private DailyRewardsCalendar _calendar;
        private IViewDataProvider<CollectableInfo, CollectableViewData> _viewDataProvider;

        private ICollector<Collectable, CollectableCollectorArgs> _collector;

        public DailyRewards(
                    IViewDataProvider<CollectableInfo, CollectableViewData> viewDataProvider,
                    IConfig<int, Collectable> dailyRewardConfig)
        {
            _viewDataProvider = viewDataProvider;
            _dailyRewardConfig = dailyRewardConfig;
        }

        public void Setup(DailyRewardsCalendar calendar)
        {
            _calendar = calendar;

            for (int i = 0; i < _daysInCalendar; i++)
            {
                var reward = _dailyRewardConfig.GetData(i);
                var view = _viewDataProvider.GetViewData(reward.Info);
                _calendar.SetupViews(i, view, reward);
            }

            _nextRewardAvailable = DateTime.UtcNow.AddSeconds(_nextRewardDuration);
        }

        public void Collect()
        {
            //
            var reward = _dailyRewardConfig.GetData(_currentCollectedDay);
            var view = _calendar.GetView(_currentCollectedDay);
            _collector.Collect(reward, new CollectableCollectorArgs
            {
                View = view
            });

            _currentCollectedDay = (_currentCollectedDay + 1) % _daysInCalendar;
            _nextRewardAvailable = DateTime.UtcNow.AddSeconds(_nextRewardDuration); // duplication

            view.Collect();

            if (_currentCollectedDay == 0)
            {
                // when all finished create new views
            }
        }

        public void Tick()
        {
            var isAvailable = DateTime.UtcNow > _nextRewardAvailable;
            var leftTime = DateTime.UtcNow - _nextRewardAvailable;
            _calendar.UpdateInfo(leftTime, isAvailable);
        }
    }
}