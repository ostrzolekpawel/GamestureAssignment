using DG.Tweening;
using GamestureAssignment.Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class CollectableHud : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;

        private CollectableInfo _info;

        public CollectableInfo Info => _info;
        public Sprite IconSprite => _icon.sprite;
        public Vector3 IconWorldPosition => _icon.rectTransform.position;

        public void Setup(CollectableInfo info, CollectableViewData viewData)
        {
            _info = info;
            _icon.sprite = viewData.Icon;
        }

        public void UpdateInfo(Collectable previous, Collectable current)
        {
            _amount.text = current.Amount.ToString();

            int previousAmount = previous.Amount;
            int currentAmount = current.Amount;

            DOTween.Kill(_amount, complete: false);

            DOVirtual.Int(
                previousAmount,
                currentAmount,
                0.3f, // duration
                value => _amount.text = value.ToString()
            ).SetEase(Ease.OutQuad)
             .SetTarget(_amount);
        }
    }
}