using System.Collections.Generic;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Collectables;
using UnityEngine;

namespace GamestureAssignment.UIs
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private CollectableHud _collectablePrefab;
        [SerializeField] private Transform _collectableParent;

        private readonly List<CollectableHud> _views = new();

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
    }
}