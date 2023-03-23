using UnityEngine;

public abstract class ShieldControllerBase : MonoBehaviour,IShield
{
    [SerializeField] protected Collider2D unitCollider;
    [SerializeField] protected GameObject shields;
    [SerializeField] protected Collider2D shieldCollider;

    public bool shieldsActive;
    [SerializeField] protected bool shieldsActiveOnSpawn;

    public bool reflectorShield;

    protected void Start()
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
            Debug.Log("Shield on ship collision");
            damageable.Destroy();
            DeactivateShields();
        }

        else
        {
            if (collider.TryGetComponent<IShield>(out var shield))
            {
                Debug.Log("Shield on shield collision");
                shield.DeactivateShields();
                DeactivateShields();
            }
        }
    }


    public virtual void ReduceShields()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ReflectProjectile(GameObject objectToReflect)
    {
        objectToReflect.transform.right = transform.right;
    }
}
