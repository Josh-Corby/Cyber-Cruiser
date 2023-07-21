using UnityEngine;

namespace CyberCruiser
{
    public class InstantiatedObject : MonoBehaviour
    {
        private void OnEnable()
        {
            AnimatedPanelController.OnGameplayPanelClosed += DestroyParticle;
        }

        private void OnDisable()
        {
            AnimatedPanelController.OnGameplayPanelClosed -= DestroyParticle;
        }

        private void DestroyParticle()
        {
            Destroy(gameObject);
        }
    }
}
