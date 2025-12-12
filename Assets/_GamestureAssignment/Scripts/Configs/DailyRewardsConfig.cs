using System.Collections.Generic;
using System.Linq;
using GamestureAssignment.Collectables;
using UnityEngine;

namespace GamestureAssignment.Configs
{
    [CreateAssetMenu(fileName = "DailyRewardsConfig", menuName = "GamestureAssignment/Configs/DailyRewardsConfig")]
    public class DailyRewardsConfig : ScriptableObject
    {
        [SerializeField] private CollectableCatalog _catalog;
        [SerializeField] private List<Collectable> _rewards;

        public IReadOnlyList<Collectable> Rewards => _rewards;

        private void OnValidate()
        {
            foreach (var reward in _rewards)
            {
                if (!_catalog.AllCollectables.Contains(reward.Info))
                {
                    Debug.LogError($"Daily reward uses invalid collectable: {reward.Info.Type} {reward.Info.Code}");
                }
            }
        }
    }
}