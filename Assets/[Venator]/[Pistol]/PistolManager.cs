using System.Collections;
using UnityEngine;

public class PistolManager : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    //--------------
    private AudioSource audioSource;
    private ParticleSystem muzzleFlashPS;
    private Animator animator;

    [Header("Settings")]
    public float projectileSpeed = 20f;
    public int damage = 25;
    public float interval = 2f;

    private bool canShoot = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        muzzleFlashPS = GetComponent<ParticleSystem>();
    }

    public void Fire()
    {
        Debug.Log("Fire()");
        if (canShoot)
        {
            Debug.Log("FIREEEE");
            if (projectilePrefab == null || firePoint == null) return;

            // Play animation, SFX and FX
            if (animator) animator.Play("Shoot");
            if (audioSource) audioSource.Play();
            if (muzzleFlashPS) muzzleFlashPS.Play();

            // Create projectile
            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Initialize projectile
            LaserProjectile laserScript = newProjectile.GetComponent<LaserProjectile>();
            if (laserScript != null)
            {
                laserScript.Initialize(firePoint.forward, projectileSpeed, damage);
            }

            canShoot = false;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (animator) animator.Play("Reload");

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(interval);

        Debug.Log("Reloaded");
        canShoot = true;
    }
}