using System.Collections;
using UnityEngine;

public class PistolManager : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public GameObject pistolHand;
    public Animator mainAnim;
    public Animator modelAnim;
    public Animator handAnim;

    [Header("Audio")]
    public AudioClip shootSFX;
    public AudioClip reloadSFX;


    //--------------
    private AudioSource audioSource;
    private ParticleSystem[] muzzleFlashPS;

    [Header("Settings")]
    public float projectileSpeed = 20f;
    public int damage = 25;
    public float interval = 2f;

    private bool canShoot = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        muzzleFlashPS = GetComponentsInChildren<ParticleSystem>();
    }

    public void Fire()
    {
        Debug.Log("Fire()");
        if (canShoot)
        {
            Debug.Log("FIREEEE");
            if (projectilePrefab == null || firePoint == null) return;

            // Play animation, SFX and FX
            if (audioSource) StartCoroutine(PlaySFX());
            if (mainAnim) mainAnim.SetTrigger("shoot");
            if (modelAnim) modelAnim.SetTrigger("shoot");
            if (handAnim) handAnim.SetTrigger("shoot");

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
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(interval);

        Debug.Log("Reloaded");
        canShoot = true;
    }

    private IEnumerator PlaySFX()
    {
        audioSource.PlayOneShot(shootSFX);
        yield return new WaitForSeconds(.25f);
        audioSource.PlayOneShot(reloadSFX);
    }

    public void PlayPS()
    {
        if (muzzleFlashPS.Length != 0)
        {
            foreach (ParticleSystem PS in muzzleFlashPS)
                PS.Play();
        }
    }

    public void Grab()
    {
        pistolHand.SetActive(true);
    }
    
    public void Ungrab()
    {
        pistolHand.SetActive(false);
    }
}