using System.Threading;
using Cysharp.Threading.Tasks;

namespace GamestureAssignment.CollectableCollector
{
    public interface ICollectorProvider<in TData, in TArgs> // todo change name to collector provider?
    {
        UniTask CollectAsync(TData data, CancellationToken token, TArgs args = default);
    }
}