using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaserProjectile : MonoBehaviour
{
    private int damageAmount;
    private Rigidbody rb;
    private Collider myCollider; // Referencia a mi propio collider
    private bool isInitialized = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>(); // Guardamos referencia
    }

    // AHORA ACEPTAMOS UN NUEVO PARAMETRO: "colliderIgnorar"
    public void Initialize(Vector3 direction, float speed, int damage, Collider colliderIgnorar)
    {
        damageAmount = damage;
        isInitialized = true;

        // 1. LIMPIEZA DE FÍSICAS
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. SOLUCIÓN ROTACIÓN (Tumbamos la bala 90 grados)
        // Miramos al frente
        Quaternion rotacionMirada = Quaternion.LookRotation(direction);
        // Ajustamos 90 grados en X (prueba -90 si sale del revés)
        Quaternion ajuste = Quaternion.Euler(90f, 0f, 0f);
        transform.rotation = rotacionMirada * ajuste;

        // 3. SOLUCIÓN COLISIÓN (Ignorar a la pistola)
        // Si nos han pasado el collider de la pistola, lo ignoramos
        if (colliderIgnorar != null && myCollider != null)
        {
            Physics.IgnoreCollision(myCollider, colliderIgnorar);
        }

        // 4. MOVIMIENTO
        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, 5.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si chocamos con la pistola a pesar de todo, no hacemos nada
        if (collision.gameObject.name.Contains("Pistola")) return;

        if (!isInitialized) return;

        IHealth target = collision.gameObject.GetComponentInParent<IHealth>();

        if (target != null && target.IsVulnerable())
        {
            Debug.Log($"[BALA] IMPACTO REAL en {collision.gameObject.name}. Aplicando daño.");
            target.ApplyDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}