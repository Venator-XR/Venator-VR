using UnityEngine;

public class BatProjectile : MonoBehaviour
{

    [Header("Configuration")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    private void HandleImpact(GameObject hitObject)
    {
        if (hitObject.CompareTag("Player"))
        {
            if (hitObject.TryGetComponent(out PlayerHealth health) && health != null)
            {
                health.ApplyDamage(1);
            } else if (health == null) Debug.LogError("PlayerHealth not reached");
        }

        Destroy(gameObject);
    }
}
