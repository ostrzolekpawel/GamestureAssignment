using System.Collections.Generic;
using GamestureAssignment.Collectables;
using UnityEngine;

namespace GamestureAssignment.CollectableCollector
{
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
}