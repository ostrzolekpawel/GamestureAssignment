using Cysharp.Threading.Tasks;
using GamestureAssignment.Collectables;
using Osiris.Configs;

namespace GamestureAssignment.CollectableDisplayer
{
    public class CollectableViewDataProvider : IViewDataProvider<CollectableInfo, CollectableViewData>
    {
        private readonly IConfig<CollectableInfo, CollectableViewData> _config;

        public CollectableViewDataProvider(IConfig<CollectableInfo, CollectableViewData> config)
        {
            _config = config;
        }

        public CollectableViewData GetViewData(CollectableInfo data)
        {
            return _config.GetData(data);
        }

        public UniTask<CollectableViewData> GetViewDataAsync(CollectableInfo data)
        {
            return UniTask.FromResult(_config.GetData(data));
        }
    }
}