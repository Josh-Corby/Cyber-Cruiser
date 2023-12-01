using UnityEngine;

namespace CyberCruiser
{
    public class BurstVents : MonoBehaviour
    {
        [SerializeField] private GameObject _burstObject;

        public void Burst()
        {
            Debug.Log("Bursting");
            GameObject burstExplosion = Instantiate(_burstObject, transform.position, Quaternion.identity);
            burstExplosion.transform.parent = null;
        }
    }
}
