using GamestureAssignment.CollectableCollector;
using GamestureAssignment.Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class DailyRewardView : MonoBehaviour, ICollectableView
    {
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmount;

        private Material _iconMaterialInstance;

        public Image Image => _rewardIcon;
        public TextMeshProUGUI Text => _rewardAmount;

        private void Awake()
        {
            _iconMaterialInstance = Instantiate(_rewardIcon.material);
            _rewardIcon.material = _iconMaterialInstance;
        }

        public void Setup(CollectableViewData viewData, Collectable data)
        {
            _rewardIcon.sprite = viewData.Icon;
            _rewardAmount.text = data.Amount.ToString();

            // reset view here
            UnCollect();
        }

        public void Collect()
        {
            _rewardAmount.text = "Collected";
            SetGray(true);
        }

        private void UnCollect()
        {
            SetGray(false);
        }

        private void SetGray(bool isOn)
        {
            if (_iconMaterialInstance != null)
            {
                _iconMaterialInstance.SetFloat("_IsOn", isOn ? 1f : 0f);
            }
        }
    }
}