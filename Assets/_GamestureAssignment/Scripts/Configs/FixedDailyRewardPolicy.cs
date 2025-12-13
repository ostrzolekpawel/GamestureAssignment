using System;
using GamestureAssignment.UIs;
using UnityEngine;

namespace GamestureAssignment.Configs
{
    [CreateAssetMenu(fileName = "FixedDailyRewardPolicy", menuName = "GamestureAssignment/Configs/FixedDailyRewardPolicy")]
    public sealed class FixedDailyRewardPolicy : ScriptableObject, IDailyRewardTimePolicy
    {
        [SerializeField] private int _seconds;

        public TimeSpan GetNextRewardDelay(CalendarContext context)
        {
            return TimeSpan.FromSeconds(_seconds);
        }
    }
}