using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GamestureAssignment.Configs
{
    public interface IDailyRewardsProvider<T>
    {
        UniTask<IReadOnlyList<T>> GetDailyRewards();
    }

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