using System.Collections;
using UnityEngine;

public class DestroyAfterSound : MonoBehaviour
{
    void Update()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && !audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class LaserProjectile : MonoBehaviour
{
    [Header("Conf")]
    public float delay = 0.05f;
    private int damageAmount;
    private Rigidbody rb;
    private bool isInitialized = false;

    [Header("SFXs")]
    private AudioSource audioSource;
    public AudioClip hitSFX;
    public AudioClip vampireHitSFX;

    [Header("Impact FXs")]
    public GameObject impactFX; 
    public GameObject vampireImpactFX; 

    void Awake()
    {
        Debug.Log("projectile awake()");
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        audioSource = GetComponentInChildren<AudioSource>();
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
        StartCoroutine(DelayedProjectile(delay, direction, speed));

        // Autodestroy
        Destroy(gameObject, 5.0f);
    }

    private IEnumerator DelayedProjectile(float delay, Vector3 direction, float speed)
    {
        yield return new WaitForSeconds(delay);

        rb.linearVelocity = direction.normalized * speed;
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

        audioSource.PlayOneShot(hitSFX);
        // Impact FX
        if (impactFX != null) Instantiate(impactFX, transform.position, Quaternion.identity);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(vampireHitSFX) audioSource.PlayOneShot(vampireHitSFX);
            if(vampireImpactFX) Instantiate(vampireImpactFX, transform.position, Quaternion.identity);
        }

        if(collision.gameObject.CompareTag("Painting"))
        {
            collision.gameObject.GetComponent<PaintingShot>().Shot();
        }

        // Detach audio source before destroying bullet
        audioSource.transform.parent = null;
        audioSource.gameObject.AddComponent<DestroyAfterSound>();
        
        Destroy(gameObject);
    }
}