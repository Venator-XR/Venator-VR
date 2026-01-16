using UnityEngine;

public class BatProjectile : MonoBehaviour
{

    [Header("Configuration")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float detectionRadius = 0.25f; // Radio de la "bola" de detección
    [SerializeField] private LayerMask hitLayers; // IMPORTANTE: Configurar esto en el inspector

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


    void Update()
    {
        // 1. Mover el proyectil (Tu código de movimiento aquí)
        // Por ejemplo: transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 2. ESCÁNER MANUAL (La Fuerza Bruta)
        // Creamos una bola invisible en la posición actual y preguntamos qué toca
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, hitLayers);

        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                // Ignoramos al propio enemigo o al proyectil mismo si se detecta
                if (hit.CompareTag("Enemy") || hit.gameObject == gameObject) continue;

                // Buscamos la vida del jugador
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health == null) health = hit.GetComponentInParent<PlayerHealth>();

                if (health != null)
                {
                    Debug.Log("Impacto confirmado por OverlapSphere"); // Esto saldrá en Logcat
                    health.ApplyDamage(1); // Ojo: usa ApplyDamage o TakeDamage según tu script real

                    // Efectos visuales y destruir
                    DestroyProjectile();
                    return; // Importante salir para no golpear 2 veces en el mismo frame
                }

                // Opcional: Si quieres que se destruya con paredes (Cualquier cosa que no sea Trigger)
                else if (!hit.isTrigger)
                {
                    DestroyProjectile();
                    return;
                }
            }
        }
    }

    void DestroyProjectile()
    {
        // Tu lógica de partículas y destrucción
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.transform.parent = null;
            ps.Stop();
            Destroy(ps.gameObject, 2f);
        }
        Destroy(gameObject);
    }

    // Dibujamos la bola en el editor para que veas el tamaño
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
