using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaserProjectile : MonoBehaviour
{
    private int damageAmount;
    private Rigidbody rb;
    private bool isInitialized = false;

    [Tooltip("Sparks/explosion prefab")]
    public GameObject impactFX; 
    public GameObject vampireImpactFX; 

    void Awake()
    {
        Debug.Log("projectile awake()");
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Initialize(Vector3 direction, float speed, int damage)
    {
        damageAmount = damage;
        isInitialized = true;

        // physics reset
        rb.linearVelocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;

        // Aim rotation
        transform.rotation = Quaternion.LookRotation(direction);

        // Apply speed
        rb.linearVelocity = direction.normalized * speed;

        // Autodestroy
        Destroy(gameObject, 5.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isInitialized) return;

        IHealth target = collision.gameObject.GetComponentInParent<IHealth>();

        if (target != null)
        {
            target.ApplyDamage(damageAmount);
            Debug.Log("Hit! Damage applied");
        }

        // Impact FX
        if (impactFX != null)
        {
            Instantiate(impactFX, transform.position, Quaternion.identity);
        }

        if (collision.gameObject.CompareTag("Enemy") && vampireImpactFX != null)
        {
            Instantiate(vampireImpactFX, transform.position, Quaternion.identity);
        }

        if(collision.gameObject.CompareTag("Painting"))
        {
            collision.gameObject.GetComponent<PaintingShot>().Shot();
        }

        Destroy(gameObject);
    }
}