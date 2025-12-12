using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamestureAssignment.Collectables;

namespace GamestureAssignment.Configs
{
    public class DailyRewardsConfigProvider : IDailyRewardsProvider<Collectable>
    {
        private readonly DailyRewardsConfig _config;

        public DailyRewardsConfigProvider(DailyRewardsConfig config)
        {
            _config = config;
        }

        public UniTask<IReadOnlyList<Collectable>> GetDailyRewards()
        {
            return UniTask.FromResult<IReadOnlyList<Collectable>>(_config.Rewards);
        }
    }
}