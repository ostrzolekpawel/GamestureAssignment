using GamestureAssignment.CollectableCollector;
using GamestureAssignment.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureAssignment.UIs
{
    public class CollectableHud : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;
        [SerializeField] private ParticleSystemForceField _forceField;
        [SerializeField] private ParticleSystem _particleSystemPrefab;

        private CollectableInfo _info;

        public CollectableInfo Info => _info;

        public void Setup(CollectableInfo info, CollectableViewData viewData)
        {
            _info = info;
            _icon.sprite = viewData.Icon;
        }

        public void UpdateInfo(CollectCollectableSignal signal) // more like needed information not just signal
        {
            _amount.text = signal.Current.Amount.ToString();
            var ps = Instantiate(_particleSystemPrefab, signal.CollectStartPoint);

            ps.externalForces.AddInfluence(_forceField);
            // or just use do tween
            // can do animation based on information previous and current
        }
    }
}