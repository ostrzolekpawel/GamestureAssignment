using System;
using System.Collections.Generic;
using GamestureAssignment.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    /// <summary>
    /// controls rewards?
    /// </summary>
    public class DailyRewardsCalendar : MonoBehaviour
    {
        [SerializeField] private List<DailyRewardView> _views;
        [SerializeField] private Button _collect;
        [SerializeField] private TextMeshProUGUI _collectStatus; // change it to time / collect

        private Dictionary<int, DailyRewardView> _viewMap = new Dictionary<int, DailyRewardView>();

        public event Action Collected;

        private void Awake()
        {
            for (int i = 0; i < _views.Count; i++)
            {
                _viewMap[i] = _views[i];
            }

            _collect.onClick.AddListener(Collect);
        }

        public void SetupViews(int day, CollectableViewData viewData, Collectable reward)
        {
            if (_viewMap.TryGetValue(day, out var view))
            {
                view.Setup(viewData, reward);
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