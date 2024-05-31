using UnityEngine;

namespace CyberCruiser
{
    public class Mine : Enemy
    {
        void Start()
        {
            if (transform.rotation.eulerAngles.z != 0 && Mathf.Abs(transform.rotation.eulerAngles.z) % 90 == 0)
            {
                _spriteRenderer.gameObject.transform.Rotate(0, 0, -45);
                //_spriteRenderer.gameObject.transform.rotation = new Vector3(transform.rotation.x, transform.rotation.y - 45, transform.rotation.z);
            }
        }

    }
}
