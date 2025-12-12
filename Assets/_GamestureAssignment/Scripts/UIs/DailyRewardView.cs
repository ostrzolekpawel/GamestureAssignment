using System;
using System.Collections.Generic;
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

        private Material _iconMaterialInstance;

        public Image Image => _rewardIcon;
        public TextMeshProUGUI Text => _rewardAmount;

        private void Awake()
        {
            _iconMaterialInstance = Instantiate(_rewardIcon.material);
            _rewardIcon.material = _iconMaterialInstance;
        }

        public void Setup(CollectableViewData viewData, Collectable data)
        {
            _rewardIcon.sprite = viewData.Icon;
            _rewardAmount.text = data.Amount.ToString();

            // reset view here
            UnCollect();
        }

        public void Collect()
        {
            _rewardAmount.text = "Collected";
            SetGray(true);
        }

        private void UnCollect()
        {
            SetGray(false);
        }

        private void SetGray(bool isOn)
        {
            if (_iconMaterialInstance != null)
            {
                _iconMaterialInstance.SetFloat("_IsOn", isOn ? 1f : 0f);
            }
        }
    }

    public class DailyRewards : IDisposable // todo also create interface for daily rewards
    {
        private readonly IDailyRewardsProvider<Collectable> _dailyRewardsProvider;
        private readonly IEventBus _signalBus;
        private readonly IViewDataProvider<CollectableInfo, CollectableViewData> _viewDataProvider;
        private int _daysInCalendar = 6;
        private int _currentCollectedDay;
        private int _nextRewardDuration = 3600 * 24; // take those information from config?
        private DateTime _nextRewardAvailable;
        private DailyRewardsCalendar _calendar;
        private IReadOnlyList<Collectable> _currentDailyRewards;

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

    public class TimeCheatSignal
    {
    }

    public class HUDMediator : IDisposable
    {
        private readonly HudView _hud;
        private readonly IReadOnlyList<CollectableInfo> _collectables;
        private readonly IViewDataProvider<CollectableInfo, CollectableViewData> _viewDataProvider;
        private readonly IEventBus _bus;

        private readonly Dictionary<CollectableInfo, CollectableHud> _viewsByInfo = new Dictionary<CollectableInfo, CollectableHud>();

        public HUDMediator(HudView hud, IReadOnlyList<CollectableInfo> collectables, IViewDataProvider<CollectableInfo, CollectableViewData> viewDataProvider, IEventBus bus)
        {
            _hud = hud;
            _collectables = collectables;
            _viewDataProvider = viewDataProvider;
            _bus = bus;

            _bus.Subscribe<CollectCollectableSignal>(OnCollect);
        }

        public void Setup()
        {
            _hud.Setup(_collectables, _viewDataProvider);

            foreach (var view in _hud.Views)
            {
                Debug.Log($"Try add: {view.Info}");
                _viewsByInfo.Add(view.Info, view);
            }
        }

        private void OnCollect(CollectCollectableSignal signal)
        {
            var info = signal.Current.Info;

            if (_viewsByInfo.TryGetValue(info, out var hudView))
            {
                hudView.UpdateInfo(signal);
            }
        }

        public void Dispose()
        {
            _bus.Unsubscribe<CollectCollectableSignal>(OnCollect);
        }
    }
}