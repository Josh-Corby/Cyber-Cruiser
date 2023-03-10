using UnityEngine;

public abstract class ShieldBase : MonoBehaviour
{
    [SerializeField] protected Collider2D unitCollider;
    [SerializeField] protected GameObject shields;

    public bool shieldsActive;
    [SerializeField] protected bool shieldsActiveOnSpawn;

    private void Start()
    {
        if (shieldsActiveOnSpawn)
        {
            ActivateShields();
        }
    }

    public virtual void ActivateShields()
    {
        if (shieldsActive)
        {
            return;
        }

        shieldsActive = true;
        shields.SetActive(true);
        unitCollider.enabled = false;
    }

    public virtual void DeactivateShields()
    {
        if (!shieldsActive)
        {
            return;
        }

        shieldsActive = false;
        unitCollider.enabled = true;
        shields.SetActive(false);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {

        if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Destroy();
            DeactivateShields();
        }

        else
        {
            if (collider.TryGetComponent<IShield>(out var shield))
            {
                shield.DeactivateShields();
                DeactivateShields();
            }
        }
    }
}
