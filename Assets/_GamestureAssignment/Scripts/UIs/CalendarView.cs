using System;
using System.Collections.Generic;
using GamestureAssignment.Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class CalendarView : MonoBehaviour
    {
        [SerializeField] private DailyRewardView _dailyRewardPrefab; // todo use object pool?
        [SerializeField] private Transform _container;
        [SerializeField] private Button _collect;
        [SerializeField] private TextMeshProUGUI _collectStatus;

        private readonly Dictionary<int, DailyRewardView> _viewMap = new Dictionary<int, DailyRewardView>();

        public event Action Collected;

        private void Awake()
        {
            _collect.onClick.AddListener(Collect);
        }

        public void SetupViews(int day, CollectableViewData viewData, Collectable reward)
        {
            if (_viewMap.TryGetValue(day, out var view))
            {
                view.Setup(viewData, reward);
            }
            else
            {
                var createdView = Instantiate(_dailyRewardPrefab, _container);
                _viewMap[day] = createdView;
                createdView.Setup(viewData, reward);
            }
        }

        private void Collect()
        {
            Collected?.Invoke();
        }

        public DailyRewardView GetView(int day)
        {
            if (_viewMap.TryGetValue(day, out var view))
            {
                return view;
            }
            return null;
        }

        public void UpdateInfo(TimeSpan leftTime, bool isAvailable)
        {
            _collectStatus.text = isAvailable ? "Collect" : leftTime.ToString(@"hh\:mm\:ss");
            _collect.interactable = isAvailable;
        }

        private void OnDestroy()
        {
            _collect.onClick.RemoveListener(Collect);
        }
    }
}