using System;
using UnityEngine;

namespace GamestureAssignment.Collectables
{
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
}