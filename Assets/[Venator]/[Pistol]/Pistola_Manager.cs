using UnityEngine;
using UnityEngine.InputSystem; // Necesario para leer el gatillo directamente

public class PistolaManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject laserPrefab;
    public Transform firePoint;

    [Header("Configuraci�n")]
    public float projectileSpeed = 20f;
    public int damage = 25;

    [Header("Input de Disparo (NUEVO)")]
    // Arrastra aqu� la acci�n "Activate" (Gatillo)
    [SerializeField] private InputActionProperty shootInput;

    private Collider myGunCollider;

    void Start()
    {
        // Buscamos collider para evitar auto-disparo
        myGunCollider = GetComponent<Collider>();
        if (myGunCollider == null) myGunCollider = GetComponentInChildren<Collider>();

        // Activar el input si es necesario
        if (shootInput.action != null) shootInput.action.Enable();
    }

    void Update()
    {
        // AHORA DETECTAMOS EL DISPARO AQU� DIRECTAMENTE
        // WasPressedThisFrame = Solo dispara una vez por clic (semi-autom�tica)
        if (shootInput.action != null && shootInput.action.WasPressedThisFrame())
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (laserPrefab == null || firePoint == null) return;

        // Crear bala
        GameObject newProjectile = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        LaserProjectile laserScript = newProjectile.GetComponent<LaserProjectile>();

        if (laserScript != null)
        {
            // Inicializar bala ignorando mi propio collider
            laserScript.Initialize(firePoint.forward, projectileSpeed, damage, myGunCollider);
        }
    }
}