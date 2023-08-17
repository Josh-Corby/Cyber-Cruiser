using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class DistanceText : MonoBehaviour
    {
        private TMP_Text _text;
        [SerializeField] private DistanceManager _distanceManager;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            _text.text = _distanceManager.DistanceInt.ToString() + "m";
        }
    }
}
