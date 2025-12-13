using System.Collections.Generic;
using DG.Tweening;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Collectables;
using UnityEngine;

namespace GamestureAssignment.UIs
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private CollectableHud _collectablePrefab;
        [SerializeField] private Transform _collectableParent;
        [SerializeField] private CollectableHudFlyIcon _flyIconPrefab;
        [SerializeField] private RectTransform _flyIconParent;

        private readonly List<CollectableHud> _views = new List<CollectableHud>();

        public IReadOnlyList<CollectableHud> Views => _views;

        public void Setup(
            IReadOnlyList<CollectableInfo> collectables,
            IViewDataProvider<CollectableInfo, CollectableViewData> viewDataProvider)
        {
            foreach (var info in collectables)
            {
                var view = Instantiate(_collectablePrefab, _collectableParent);

                var viewData = viewDataProvider.GetViewData(info);
                view.Setup(info, viewData);

                _views.Add(view);
            }
        }

        public void Collect(CollectCollectableSignal signal, CollectableHud collectableHud)
        {
            var flyIcon = GameObject.Instantiate(
                _flyIconPrefab, _flyIconParent
            );

            flyIcon.SetIcon(collectableHud.IconSprite);

            RectTransform start = signal.CollectStartPoint;
            RectTransform rt = flyIcon.RectTransform;

            rt.position = start.position;

            Vector3 targetPos = collectableHud.IconWorldPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                rt.DOJump(
                    targetPos,
                    jumpPower: 40f,
                    numJumps: 1,
                    duration: 0.45f
                ).SetEase(Ease.OutQuad)
            );

            sequence.OnComplete(() => GameObject.Destroy(flyIcon.gameObject));
        }
    }
}