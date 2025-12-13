using System.Collections.Generic;
using GamestureAssignment.CollectableCollector;
using GamestureAssignment.CollectableDisplayer;
using GamestureAssignment.Collectables;
using OsirisGames.EventBroker;

namespace GamestureAssignment.UIs
{
    public class HudMediator : IHudMediator
    {
        private readonly HudView _hud;
        private readonly IReadOnlyList<CollectableInfo> _collectables;
        private readonly IViewDataProvider<CollectableInfo, CollectableViewData> _viewDataProvider;
        private readonly IEventBus _bus;

        private readonly Dictionary<CollectableInfo, CollectableHud> _viewsByInfo = new Dictionary<CollectableInfo, CollectableHud>();

        public HudMediator(HudView hud, IReadOnlyList<CollectableInfo> collectables, IViewDataProvider<CollectableInfo, CollectableViewData> viewDataProvider, IEventBus bus)
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
                _viewsByInfo.Add(view.Info, view);
            }
        }

        private void OnCollect(CollectCollectableSignal signal)
        {
            var info = signal.Current.Info;

            if (_viewsByInfo.TryGetValue(info, out var hudView))
            {
                hudView.UpdateInfo(signal.Previous, signal.Current);
                _hud.Collect(signal, hudView);
            }
        }

        public void Dispose()
        {
            _bus.Unsubscribe<CollectCollectableSignal>(OnCollect);
        }
    }
}