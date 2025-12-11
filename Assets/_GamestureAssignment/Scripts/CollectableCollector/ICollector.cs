using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamestureAssignment.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.CollectableCollector
{
    public interface ICollector<in TData, in TArgs> // todo change name to collector provider?
    {
        void Collect(TData data, TArgs args = default);
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

    public class DefaultCollectableCollector : ICollector<Collectable, CollectableCollectorArgs>
    {
        private readonly IInventory<Collectable, CollectableInfo> _inventory;

        public DefaultCollectableCollector(IInventory<Collectable, CollectableInfo> inventory)
        {
            _inventory = inventory;
        }

        public void Collect(Collectable data, CollectableCollectorArgs args = null)
        {
            _inventory.Add(data);
        }

        public UniTask CollectAsync(Collectable data, CancellationToken token, CollectableCollectorArgs args = null)
        {
            _inventory.Add(data);

            return UniTask.CompletedTask;
        }
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
        }

        public void Set(Collectable element)
        {
            var collectable = TakeElement(element.Info);
            collectable.Amount = element.Amount;
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