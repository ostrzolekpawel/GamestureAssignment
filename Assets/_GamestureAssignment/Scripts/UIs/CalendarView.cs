using System;
using System.Collections.Generic;
using DG.Tweening;
using GamestureAssignment.Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class CalendarView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DailyRewardView _dailyRewardPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private Button _collect;
        [SerializeField] private TextMeshProUGUI _collectStatus;

        [Header("Animation")]
        [SerializeField] private float _spawnDuration = 0.25f;
        [SerializeField] private float _finishDuration = 0.2f;
        [SerializeField] private float _finishStagger = 0.05f;

        private ObjectPool<DailyRewardView> _pool;

        private readonly List<DailyRewardView> _activeViews = new();
        private readonly Dictionary<int, DailyRewardView> _viewMap = new();

        public event Action Collected;

        private void Awake()
        {
            _collect.onClick.AddListener(OnCollect);

            _pool = new ObjectPool<DailyRewardView>(
                CreateView,
                OnGetView,
                OnReleaseView,
                DestroyView,
                collectionCheck: false
            );
        }

        private DailyRewardView CreateView()
        {
            return Instantiate(_dailyRewardPrefab, _container);
        }

        private void OnGetView(DailyRewardView view)
        {
            view.gameObject.SetActive(true);
            view.transform.localScale = Vector3.zero;
        }

        private void OnReleaseView(DailyRewardView view)
        {
            view.transform.DOKill();
            view.gameObject.SetActive(false);
        }

        private void DestroyView(DailyRewardView view)
        {
            Destroy(view.gameObject);
        }

        public void SetupViews(int day, CollectableViewData viewData, Collectable reward)
        {
            if (!_viewMap.TryGetValue(day, out var view))
            {
                view = _pool.Get();
                _viewMap[day] = view;
                _activeViews.Add(view);
            }

            view.Setup(viewData, reward);

            view.transform
                .DOScale(1f, _spawnDuration)
                .SetEase(Ease.OutBack);
        }

        public DailyRewardView GetView(int day)
        {
            _viewMap.TryGetValue(day, out var view);
            return view;
        }

        public void UpdateInfo(TimeSpan leftTime, bool isAvailable)
        {
            _collectStatus.text =
                isAvailable ? "Collect" : leftTime.ToString(@"hh\:mm\:ss");

            _collect.interactable = isAvailable;
        }

        public void Finished()
        {
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < _activeViews.Count; i++)
            {
                var view = _activeViews[i];

                sequence.Append(
                    view.transform
                        .DOScale(0f, _finishDuration)
                        .SetEase(Ease.InBack)
                );

                if (_finishStagger > 0f)
                    sequence.AppendInterval(_finishStagger);
            }

            sequence.OnComplete(ClearViews);
        }

        private void ClearViews()
        {
            foreach (var view in _activeViews)
            {
                _pool.Release(view);
            }

            _activeViews.Clear();
            _viewMap.Clear();
        }

        private void OnCollect()
        {
            Collected?.Invoke();
        }

        private void OnDestroy()
        {
            _collect.onClick.RemoveListener(OnCollect);
            _pool.Clear();
        }
    }
}
