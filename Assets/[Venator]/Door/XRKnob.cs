using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

// Si usas XRI versión antigua (pre-2.0), comenta la línea de arriba y descomenta esta:
// using UnityEngine.XR.Interaction.Toolkit.Interactables; 

public class XRKnob : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    [Header("Configuración Visual")]
    [Tooltip("El objeto visual que girará (la manivela)")]
    public Transform handle;
    
    [Header("Valores")]
    [Range(0f, 1f)]
    public float value = 0.0f;
    public bool clamped = true; // Si es false, gira infinito
    public float maxAngle = 180f; // Ángulo máximo si está 'clamped'

    [Header("Eventos")]
    public UnityEvent<float> onValueChange;

    private float _startAngle = 0.0f;
    private Quaternion _startRotation;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor _interactor;

    protected override void Awake()
    {
        base.Awake();
        if (handle == null) handle = transform; // Si no asignas nada, gira el propio objeto
        _startRotation = handle.localRotation;
        UpdateRotation(value); // Coloca la manivela en su posición inicial
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _interactor = args.interactorObject;
        // Calculamos el ángulo inicial de agarre para que no salte de golpe
        _startAngle = GetInteractorAngle() - (value * maxAngle); 
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _interactor = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (isSelected && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            float currentAngle = GetInteractorAngle();
            float angleDiff = Mathf.DeltaAngle(_startAngle, currentAngle);
            
            // Convertimos la diferencia de ángulo en un valor de 0 a 1
            float newValue = angleDiff / maxAngle;

            if (clamped)
                newValue = Mathf.Clamp01(newValue);
            else
                newValue = Mathf.Repeat(newValue, 1.0f);

            // Solo actualizamos si el valor ha cambiado significativamente
            if (Mathf.Abs(newValue - value) > 0.001f)
            {
                value = newValue;
                UpdateRotation(value);
                onValueChange.Invoke(value);
            }
        }
    }

    private void UpdateRotation(float val)
    {
        // Gira alrededor del eje Z local (puedes cambiar Vector3.forward por .up o .right según tu modelo)
        float targetAngle = val * maxAngle;
        handle.localRotation = _startRotation * Quaternion.Euler(0, 0, targetAngle);
    }

    private float GetInteractorAngle()
    {
        if (_interactor == null) return 0f;

        // Proyectamos la posición de la mano sobre el plano local del objeto
        Vector3 handPos = _interactor.transform.position;
        Vector3 localHandPos = transform.InverseTransformPoint(handPos);
        
        // Usamos Atan2 para obtener el ángulo en grados (Ejes X e Y locales)
        return Mathf.Atan2(localHandPos.y, localHandPos.x) * Mathf.Rad2Deg;
    }
}