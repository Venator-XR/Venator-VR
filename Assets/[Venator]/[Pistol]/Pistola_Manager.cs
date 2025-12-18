using UnityEngine;

public class PistolaManager : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float damage = 25f;

    // Guardaremos el collider de la pistola aquí
    private Collider myGunCollider;

    void Start()
    {
        // Buscamos el collider en este mismo objeto o en los hijos (por si tienes la malla separada)
        myGunCollider = GetComponent<Collider>();
        if (myGunCollider == null)
        {
            myGunCollider = GetComponentInChildren<Collider>();
        }
    }

    public void Fire()
    {
        if (laserPrefab == null || firePoint == null) return;

        GameObject newProjectile = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        LaserProjectile laserScript = newProjectile.GetComponent<LaserProjectile>();

        if (laserScript != null)
        {
            // PASAMOS 'myGunCollider' AL FINAL
            laserScript.Initialize(firePoint.forward, projectileSpeed, damage, "Plasma", myGunCollider);
        }
    }
}