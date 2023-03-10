using UnityEngine;

public abstract class ShieldControllerBase : MonoBehaviour
{
    protected Collider2D unitCollider;
    [SerializeField] protected GameObject shields;
    [SerializeField] protected Collider2D shieldCollider;

    public bool shieldsActive;
    [SerializeField] protected bool shieldsActiveOnSpawn;

    private void Start()
    {
        if (shieldsActiveOnSpawn)
        {
            ActivateShields();
        }
        else
        {
            DeactivateShields();
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
        shieldCollider.enabled = true;
        unitCollider.enabled = false;
    }

    public virtual void DeactivateShields()
    {
        if (!shieldsActive)
        {
            return;
        }

        shieldsActive = false;
        shields.SetActive(false);
        shieldCollider.enabled = false;
        unitCollider.enabled = true;   
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {

        if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            Debug.Log(collider.name);
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
