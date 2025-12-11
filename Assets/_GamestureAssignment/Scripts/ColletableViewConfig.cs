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
        Money,
        Gold,
        Diamonds,
        Chest,
        Gift
    }

    [Serializable]
    public class Collectable
    {
        [SerializeField] private CollectableInfo _info;
        [SerializeField] private int _amount;

        public CollectableInfo Info => _info;
        public int Amount => _amount;
    }

    [Serializable]
    public struct CollectableInfo // TODO drawer because money, gold don't need code and for other can show predefined menu dropdown
    {
        [SerializeField] private CollectableType _type;
        [SerializeField] private string _code;

        public CollectableType Type => _type;
        public string Code => _code;
    }

    public enum CollectableSize
    {
        Small,
        Medium,
        Large
    }

    [Serializable]
    public class CollectableViewData
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _translationKey;

        public Sprite Icon => _icon;
        public string TranslationKey => _translationKey;
    }
}