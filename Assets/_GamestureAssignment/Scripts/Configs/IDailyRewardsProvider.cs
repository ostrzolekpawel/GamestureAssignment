using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GamestureAssignment.Configs
{
    public interface IDailyRewardsProvider<T>
    {
        UniTask<IReadOnlyList<T>> GetDailyRewards();
    }
}