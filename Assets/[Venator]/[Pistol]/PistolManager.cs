using UnityEngine;

public class PistolManager : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public AudioSource audioSource;
    public ParticleSystem muzzleFlash;

    [Header("Settings")]
    public float projectileSpeed = 20f;
    public int damage = 25;

    public void Fire()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Play SFX and FX
        if (audioSource) audioSource.Play();
        if (muzzleFlash) muzzleFlash.Play();

        // Create projectile
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // Initialize projectile
        LaserProjectile laserScript = newProjectile.GetComponent<LaserProjectile>();
        if (laserScript != null)
        {
            laserScript.Initialize(firePoint.forward, projectileSpeed, damage);
        }
    }
}