using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Osiris.Configs;
using UnityEditorInternal;

namespace GamestureAssignment.Configs
{
    public interface IDailyRewardProvider<T>
    {
        UniTask<T> GetData(int day);
    }

    public interface IDailyRewardsProvider<T>
    {
        UniTask<IConfig<int, T>> GetDailyRewards(); // todo maybe just return list
        UniTask<List<T>> GetDailyRewardsCollection(); // todo maybe just return list
    }

    public class ConfigDailyRewardProvider : IDailyRewardProvider<Collectable>
    {
        private IConfig<int, Collectable> _config; // or make list

        public ConfigDailyRewardProvider(IConfig<int, Collectable> config)
        {
            _config = config;
        }

        public UniTask<Collectable> GetData(int day)
        {
            return UniTask.FromResult(_config.GetData(day));
        }
    }

    public class ConfigDailyRewardsProvider : IDailyRewardsProvider<Collectable>
    {
        private readonly IConfig<int, Collectable> _config;
        private readonly int _calendarDays;

        public ConfigDailyRewardsProvider(IConfig<int, Collectable> config, int calendarDays)
        {
            _config = config;
            _calendarDays = calendarDays;
        }

        public UniTask<IConfig<int, Collectable>> GetDailyRewards()
        {
            return UniTask.FromResult(_config);
        }

        public UniTask<List<Collectable>> GetDailyRewardsCollection()
        {
            var collection = new List<Collectable>();
            for (int i = 0; i < _calendarDays; i++)
            {
                collection.Add(_config.GetData(i));
            }

            return UniTask.FromResult(collection);
        }
    }
}