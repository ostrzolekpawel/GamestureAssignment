using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamestureAssignment.Configs;
using OsirisGames.EventBroker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.CollectableCollector
{
    public interface ICollector<in TData, in TArgs> // todo change name to collector provider?
    {
        UniTask CollectAsync(TData data, CancellationToken token, TArgs args = default);
    }

    public interface ICollectableView
    {
        Image Image { get; }
        TextMeshProUGUI Text { get; }
    }

    public class CollectableCollectorArgs
    {
        public ICollectableView View { get; set; }
    }

    public class InventoryCollectableCollector : ICollector<Collectable, CollectableCollectorArgs>
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

            // todo check if there is view from args
            _signalBus.Fire(new CollectCollectableSignal(previous, current, (RectTransform)args.View.Image.transform));
            return UniTask.CompletedTask;
        }
    }

    public class CollectCollectableSignal
    {
        public CollectCollectableSignal(Collectable previous, Collectable current, RectTransform collectStartPoint)
        {
            Previous = previous;
            Current = current;
            CollectStartPoint = collectStartPoint;
        }

        public Collectable Previous { get; }
        public Collectable Current { get; }
        public RectTransform CollectStartPoint { get; }
    }

    public interface IInventory<TData, TType>
    {
        TData Get(TType type);
        void Add(TData element);
        void Remove(TData element);
        void Set(TData element);
    }

    public class DefaultInventory : IInventory<Collectable, CollectableInfo>
    {
        private readonly Dictionary<CollectableInfo, Collectable> _collectables = new Dictionary<CollectableInfo, Collectable>();

        public void Add(Collectable element)
        {
            var collectable = TakeElement(element.Info);
            collectable.Amount += element.Amount;
            _collectables[element.Info] = collectable;
            Debug.Log(collectable.ToString());
        }

        public Collectable Get(CollectableInfo type)
        {
            return TakeElement(type);
        }

        public void Remove(Collectable element)
        {
            var collectable = TakeElement(element.Info);
            collectable.Amount -= element.Amount;
            _collectables[element.Info] = collectable;
        }

        public void Set(Collectable element)
        {
            var collectable = TakeElement(element.Info);
            collectable.Amount = element.Amount;
            _collectables[element.Info] = collectable;
        }

        private Collectable TakeElement(CollectableInfo info)
        {
            if (!_collectables.TryGetValue(info, out var consumable))
            {
                _collectables[info] = new Collectable(info, 0);
            }

            return _collectables[info];
        }
    }

    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject _collectablePrefab;
        // todo create list of possible elements?

    }

    public interface IInventoryPresenter
    {

    }
}