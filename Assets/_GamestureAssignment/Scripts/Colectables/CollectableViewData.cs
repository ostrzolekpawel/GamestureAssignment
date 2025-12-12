using System;
using UnityEngine;

namespace GamestureAssignment.Collectables
{
    [Serializable]
    public class CollectableViewData
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;

        public Sprite Icon => _icon;
        public string Name => _name;
    }
}