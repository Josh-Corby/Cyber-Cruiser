using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class AddOnUIManager : GameBehaviour
    {
        [SerializeField] private PlayerAddOnManager _addOnManager;
        [SerializeField] private AddOnButton[] _addOnButtons;
        [SerializeField] private TMP_Text _addOnName;
        [SerializeField] private TMP_Text _addOnDescription;
        private AddOnScriptableObject _currentAddon;

        private void OnEnable()
        {
            AddOnButton.OnMouseEnter += SetCurrentAddOn;
            AddOnButton.OnMouseExit += ClearAddOn;
        }

        private void OnDisable()
        {
            AddOnButton.OnMouseEnter -= SetCurrentAddOn;
            AddOnButton.OnMouseExit -= ClearAddOn;
        }

        private void SetCurrentAddOn(AddOnScriptableObject addOn)
        {
            _currentAddon = addOn;
            SetAddOnUI();
        }

        private void SetAddOnUI()
        {
            _addOnName.text = _currentAddon.Name;
            _addOnDescription.text = _currentAddon.Description;
        }

        private void ClearAddOn()
        {
            _currentAddon = null;
            ClearAddOnUI();
        }

        private void ClearAddOnUI()
        {
            _addOnName.text = "";
            _addOnDescription.text = "";
        }

        public void SetButtonStates()
        {
            for (int i = 0; i < _addOnManager.AddOnActiveStates.Count; i++)
            {
                _addOnButtons[i]._doesPlayerHaveAddOn = _addOnManager.AddOnActiveStates[i].IsAddOnActive;
            }
        }
    }
}