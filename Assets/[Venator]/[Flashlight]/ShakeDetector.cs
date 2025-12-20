using UnityEngine;
using UnityEngine.Events;

public class ShakeDetector : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Fuerza necesaria para detectar el agite. Prueba con valores entre 2.0 y 5.0")]
    public float shakeThreshold = 3.5f;
    [Tooltip("Tiempo mínimo entre sacudidas para evitar parpadeos")]
    public float cooldownTime = 0.5f;

    [Header("Eventos")]
    public UnityEvent OnShake;

    // Variables internas para el cálculo
    private Vector3 lastPosition;
    private Vector3 lastVelocity;
    private float lastShakeTime;

    void Start()
    {
        lastPosition = transform.position;
        lastVelocity = Vector3.zero;
    }

    void Update()
    {
        // 1. Calculamos la velocidad actual basándonos en cuánto se ha movido
        // (Esto funciona aunque sea Kinematic, porque leemos la posición real del mundo)
        Vector3 currentVelocity = (transform.position - lastPosition) / Time.deltaTime;

        // 2. Calculamos la aceleración (cambio de velocidad)
        Vector3 acceleration = (currentVelocity - lastVelocity) / Time.deltaTime;

        // 3. Detectamos el "Gesto"
        // Si la aceleración es muy fuerte y ha pasado el tiempo de cooldown...
        if (acceleration.magnitude > shakeThreshold && Time.time > lastShakeTime + cooldownTime)
        {
            Debug.Log("[SHAKE] ¡Agitación detectada!");
            OnShake.Invoke();
            lastShakeTime = Time.time;
        }

        // 4. Guardamos datos para el siguiente frame
        lastPosition = transform.position;
        lastVelocity = currentVelocity;
    }
}