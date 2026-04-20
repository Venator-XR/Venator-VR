using UnityEngine;

public class ShootParticlesController : MonoBehaviour
{
    private PistolManager pistolManager;

    void Awake()
    {
        pistolManager = GetComponentInParent<PistolManager>();
    }

    public void Shoot()
    {
        pistolManager.PlayPS();
    }
}
