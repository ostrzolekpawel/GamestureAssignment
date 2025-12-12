using TMPro;
using UnityEngine.UI;

namespace GamestureAssignment.CollectableCollector
{
    public interface ICollectableView
    {
        Image Image { get; }
        TextMeshProUGUI Text { get; }
    }
}