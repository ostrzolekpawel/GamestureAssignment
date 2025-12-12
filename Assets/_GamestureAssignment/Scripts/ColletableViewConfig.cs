using System;
using Osiris.Configs;
using UnityEngine;

namespace GamestureAssignment.Configs
{
    [CreateAssetMenu(fileName = "CollectableViewData", menuName = "GamestureAssignment/Configs/CollectableViewData")]
    public class ColletableViewConfig : ConfigScriptable<CollectableInfo, CollectableViewData>
    {

    }

    public enum CollectableType
    {
        Corn,
        Wood,
        Rock,
        Gem
    }

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

    [Serializable]
    public struct CollectableInfo
    {
        [SerializeField] private CollectableType _type;
        [SerializeField] private string _code;

        public CollectableType Type => _type;
        public string Code => _code;

        public CollectableInfo(CollectableType type, string code)
        {
            _type = type;
            _code = code;
        }

        public override string ToString()
        {
            return $"[{Type}, {Code}]: ";
        }
    }

    public enum CollectableSizeType
    {
        Small,
        Medium,
        Large
    }

    public enum CollectableRarityType
    {
        Common,
        Rare,
        Epic
    }

    [Serializable]
    public class CollectableViewData
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;

        public Sprite Icon => _icon;
        public string Name => _name;
    }
}