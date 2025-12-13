using Cysharp.Threading.Tasks;

namespace GamestureAssignment.CollectableDisplayer
{
    public interface IViewDataProvider<in TData, TViewData>
    {
        TViewData GetViewData(TData data);
    }
}