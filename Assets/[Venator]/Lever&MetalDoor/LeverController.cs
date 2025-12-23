using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(HingeJoint))]
public class LeverController : MonoBehaviour, IXRSelectFilter
{
    [Header("Configuraci�n de �ngulos")]
    [SerializeField] private float angleOn = 45f;  // �ngulo de activaci�n
    [SerializeField] private float angleOff = -45f; // �ngulo de reposo

    [Header("Restricciones")]
    [SerializeField] private float maxGrabDistance = 0.3f; // Distancia m�xima para agarrar (solo direct interactors)
    [SerializeField] private float maxPullDistance = 0.15f; // Distancia m�xima que se puede alejar del punto de anclaje

    [Header("Eventos")]
    public UnityEvent OnLeverActivated; // IMPORTANTE: Conecta aqu� tu futura animaci�n <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    private XRGrabInteractable interactable;
    private HingeJoint hinge;
    private Rigidbody rb;
    private Transform baseTransform;
    private Vector3 anchorPosition;
    private bool isLocked = false;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        hinge = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();
        
        // Encontrar la Base (el objeto padre o el ConnectedBody)
        if (hinge.connectedBody != null)
        {
            baseTransform = hinge.connectedBody.transform;
        }
        else
        {
            // Buscar en el padre
            Transform parent = transform.parent;
            if (parent != null)
            {
                Rigidbody parentRb = parent.GetComponentInChildren<Rigidbody>();
                if (parentRb != null && parentRb.isKinematic)
                {
                    baseTransform = parentRb.transform;
                }
            }
        }
        
        // Guardar la posici�n inicial del anchor
        anchorPosition = transform.position;
        
        // Agregar este script como filtro de selecci�n
        // Esto previene que ray interactors puedan seleccionar el objeto
        var filters = interactable.startingSelectFilters;
        if (!filters.Contains(this))
        {
            filters.Add(this);
        }
    }
    
    // Implementaci�n de IXRSelectFilter para rechazar ray interactors
    public bool canProcess => true;
    
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        // Rechazar completamente los ray interactors
        if (interactor is XRRayInteractor)
        {
            return false;
        }
        
        // Para direct interactors, verificar distancia
        if (interactor is XRDirectInteractor)
        {
            if (interactor.transform != null)
            {
                float distance = Vector3.Distance(interactor.transform.position, transform.position);
                return distance <= maxGrabDistance;
            }
        }
        
        // Permitir otros tipos de interactors (como hand interactors) si est�n cerca
        if (interactor.transform != null)
        {
            float distance = Vector3.Distance(interactor.transform.position, transform.position);
            return distance <= maxGrabDistance;
        }
        
        // Por defecto, permitir si no podemos verificar distancia
        return true;
    }
    
    void FixedUpdate()
    {
        // Si est� siendo agarrado, limitar la distancia desde el punto de anclaje del HingeJoint
        if (interactable.isSelected && baseTransform != null && !isLocked)
        {
            // Obtener la posici�n del anchor del HingeJoint en espacio mundial
            Vector3 anchorWorldPosition = baseTransform.TransformPoint(hinge.connectedAnchor);
            Vector3 currentPosition = transform.TransformPoint(hinge.anchor);
            
            // Calcular la distancia desde el anchor
            float distanceFromAnchor = Vector3.Distance(currentPosition, anchorWorldPosition);
            
            // Si se aleja demasiado del anchor, aplicar fuerza para traerlo de vuelta
            if (distanceFromAnchor > maxPullDistance)
            {
                Vector3 directionToAnchor = (anchorWorldPosition - currentPosition).normalized;
                float excessDistance = distanceFromAnchor - maxPullDistance;
                
                // Aplicar fuerza hacia el anchor (m�s suave para evitar vibraciones)
                rb.AddForce(directionToAnchor * excessDistance * 30f, ForceMode.Force);
                
                // Reducir velocidad lineal y angular para evitar vibraciones
                rb.linearVelocity *= 0.9f;
                rb.angularVelocity *= 0.85f;
            }
            
            // Asegurar que el HingeJoint mantenga sus l�mites
            // Esto ayuda a prevenir que se estire demasiado
            JointLimits limits = hinge.limits;
            float currentAngle = hinge.angle;
            
            // Si el �ngulo est� fuera de los l�mites, aplicar fuerza de correcci�n
            if (currentAngle < limits.min || currentAngle > limits.max)
            {
                float targetAngle = Mathf.Clamp(currentAngle, limits.min, limits.max);
                float angleError = targetAngle - currentAngle;
                
                // Aplicar torque para corregir el �ngulo
                rb.AddTorque(transform.up * angleError * 10f, ForceMode.Force);
            }
        }
    }
    public void OnReleaseLever()
    {
        if (isLocked) return;

        float currentAngle = GetCurrentAngle();

        // Debug para ver qu� est� pasando realmente
        //Debug.Log($"�ngulo actual: {currentAngle} | Distancia a OFF: {Mathf.Abs(currentAngle - angleOff)} | Distancia a ON: {Mathf.Abs(currentAngle - angleOn)}");

        // L�GICA DE DISTANCIA: �A qui�n estoy m�s cerca?
        // Calculamos la distancia absoluta (sin signos) a cada punto
        float distToOff = Mathf.Abs(currentAngle - angleOff);
        float distToOn = Mathf.Abs(currentAngle - angleOn);

        // Si estoy m�s cerca de ON que de OFF...
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
        // Desactivamos la f�sica un momento para moverlo suavemente por c�digo
        // O simplemente usamos motores, pero mover el Spring es m�s f�cil para snapping visual
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

        // 1. Desactivar interacci�n (ya no se puede agarrar)
        interactable.enabled = false;

        // 2. Lanzar evento (Animaci�n futura, sonido, luces...)
        Debug.Log("�Palanca Activada y Bloqueada!");
        OnLeverActivated.Invoke();

        // Opcional: Cambiar material a "Encendido" o emitir un sonido de 'Clack'
    }

    // Utilidad para obtener el �ngulo limpio del HingeJoint
    private float GetCurrentAngle()
    {
        return hinge.angle;
    }
}