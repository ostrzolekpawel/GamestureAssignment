using System;
using System.Collections.Generic;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Collectables;
using OsirisGames.EventBroker;
using UnityEngine;

namespace GamestureAssignment.UIs
{
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