using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(HingeJoint))]
public class LeverController : MonoBehaviour
{
    [Header("Configuración de Ángulos")]
    [SerializeField] private float angleOn = 45f;  // Ángulo de activación (ej: abajo)
    [SerializeField] private float angleOff = -45f; // Ángulo de reposo (ej: arriba)
    [SerializeField] private float threshold = 0.5f; // 0.0 a 1.0 (50% del recorrido)

    [Header("Eventos")]
    public UnityEvent OnLeverActivated; // IMPORTANTE: Conecta aquí tu futura animación <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    private XRGrabInteractable interactable;
    private HingeJoint hinge;
    private bool isLocked = false;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        hinge = GetComponent<HingeJoint>();
    }
    public void OnReleaseLever()
    {
        if (isLocked) return;

        float currentAngle = GetCurrentAngle();

        // Debug para ver qué está pasando realmente
        //Debug.Log($"Ángulo actual: {currentAngle} | Distancia a OFF: {Mathf.Abs(currentAngle - angleOff)} | Distancia a ON: {Mathf.Abs(currentAngle - angleOn)}");

        // LÓGICA DE DISTANCIA: ¿A quién estoy más cerca?
        // Calculamos la distancia absoluta (sin signos) a cada punto
        float distToOff = Mathf.Abs(currentAngle - angleOff);
        float distToOn = Mathf.Abs(currentAngle - angleOn);

        // Si estoy más cerca de ON que de OFF...
        if (distToOn < distToOff)
        {
            StartCoroutine(SnapTo(angleOn, true)); // Ir a ON
        }
        else
        {
            StartCoroutine(SnapTo(angleOff, false)); // Volver a OFF
        }
    }

    private IEnumerator SnapTo(float targetAngle, bool lockAfter)
    {
        // Desactivamos la física un momento para moverlo suavemente por código
        // O simplemente usamos motores, pero mover el Spring es más fácil para snapping visual
        JointSpring spring = hinge.spring;
        spring.spring = 100f; // Fuerza para empujar
        spring.damper = 5f;
        spring.targetPosition = targetAngle;

        hinge.useSpring = true;
        hinge.spring = spring;

        yield return new WaitForSeconds(0.5f); // Esperar a que llegue

        if (lockAfter)
        {
            LockLever();
        }
        else
        {
            // Si volvemos a OFF, quitamos el muelle para que se sienta suelta de nuevo al agarrar
            hinge.useSpring = false;
        }
    }

    private void LockLever()
    {
        isLocked = true;

        // 1. Desactivar interacción (ya no se puede agarrar)
        interactable.enabled = false;

        // 2. Lanzar evento (Animación futura, sonido, luces...)
        Debug.Log("¡Palanca Activada y Bloqueada!");
        OnLeverActivated.Invoke();

        // Opcional: Cambiar material a "Encendido" o emitir un sonido de 'Clack'
    }

    // Utilidad para obtener el ángulo limpio del HingeJoint
    private float GetCurrentAngle()
    {
        return hinge.angle;
    }
}