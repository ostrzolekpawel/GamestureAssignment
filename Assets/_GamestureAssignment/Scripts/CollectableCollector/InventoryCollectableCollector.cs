using System.Threading;
using Cysharp.Threading.Tasks;
using GamestureAssignment.Collectables;
using OsirisGames.EventBroker;
using UnityEngine;

namespace GamestureAssignment.CollectableCollector
{
    public class InventoryCollectableCollector : ICollectorProvider<Collectable, CollectableCollectorArgs>
    {
        private readonly IInventory<Collectable, CollectableInfo> _inventory;
        private readonly IEventBus _signalBus;

        public InventoryCollectableCollector(IInventory<Collectable, CollectableInfo> inventory, IEventBus signalBus)
        {
            _inventory = inventory;
            _signalBus = signalBus;
        }

        public UniTask CollectAsync(Collectable data, CancellationToken token, CollectableCollectorArgs args = null)
        {
            var previous = _inventory.Get(data.Info);
            _inventory.Add(data);
            var current = _inventory.Get(data.Info);

            _signalBus.Fire(new CollectCollectableSignal(previous, current, (RectTransform)args.View.Image.transform));
            return UniTask.CompletedTask;
        }
    }
}