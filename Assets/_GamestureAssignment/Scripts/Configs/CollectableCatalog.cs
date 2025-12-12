using System;
using System.Collections.Generic;
using GamestureAssignment.Collectables;
using UnityEngine;

namespace GamestureAssignment.Configs
{
    [CreateAssetMenu(fileName = "CollectableCatalog", menuName = "GamestureAssignment/Configs/CollectableCatalog")]
    public class CollectableCatalog : ScriptableObject
    {
        [SerializeField] private List<CollectableInfo> _allCollectables;

        public IReadOnlyList<CollectableInfo> AllCollectables => _allCollectables;

        private void OnValidate()
        {
            if (_allCollectables.Count > 1)
            {
                foreach (var collectable in _allCollectables)
                {
                    var count = _allCollectables.FindAll(x => x.Type == collectable.Type && x.Code == collectable.Code).Count;
                    if (count > 1)
                    {
                        throw new ArgumentException($"An item with the same key ([{collectable.Type},{collectable.Code}]) has already been added.");
                    }
                }
            }
        }
    }
}