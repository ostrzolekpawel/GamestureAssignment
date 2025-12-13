using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class CollectableHudFlyIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private CanvasGroup _canvasGroup;

        public RectTransform RectTransform => (RectTransform)transform;
        public CanvasGroup CanvasGroup => _canvasGroup;

        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }
    }
}