using System;
using UnityEngine;

namespace GamestureAssignment.Collectables
{
    [Serializable]
    public struct Collectable
    {
        [SerializeField] private CollectableInfo _info;
        [SerializeField] private int _amount;

        public CollectableInfo Info => _info;
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
            }
        }

        public Collectable(CollectableType type, string code, int amount)
        {
            _info = new CollectableInfo(type, code);
            _amount = amount;
        }

        public Collectable(CollectableInfo info, int amount)
        {
            _info = info;
            _amount = amount;
        }

        public override string ToString()
        {
            return $"{Info}{_amount}";
        }
    }
}