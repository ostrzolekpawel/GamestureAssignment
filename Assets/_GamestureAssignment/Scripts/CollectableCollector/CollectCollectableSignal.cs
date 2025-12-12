using GamestureAssignment.Collectables;
using UnityEngine;

namespace GamestureAssignment.CollectableCollector
{
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
}