using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Content.Interaction;

public class LeverLogic : MonoBehaviour
{
    [Header("References")]
    public XRKnobLever knobLever;

    [Header("Config")]
    [Range(0f, 0.5f)]
    public float activationThreshold = 0.2f;
    public float snapDuration = 0.2f;

    [Header("SFX")]
    [SerializeField] private AudioClip activateSFX;
    private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent OnLeverActivated;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private bool isLocked = false;

    void OnEnable()
    {
        if (knobLever != null)
        {
            knobLever.selectExited.AddListener(OnLeverReleased);
            
            knobLever.onValueChange.AddListener(OnValueChange);
        }
    }

    void OnDisable()
    {
        if (knobLever != null)
        {
            knobLever.selectExited.RemoveListener(OnLeverReleased);
            knobLever.onValueChange.RemoveListener(OnValueChange);
        }
    }
    private void OnValueChange(float currentValue)
    {
        if (isLocked) return;

        if (currentValue <= 0.05f)
        {
            CompleteActivation();
        }
    }

    private void OnLeverReleased(SelectExitEventArgs args)
    {
        if (isLocked) return;

        // should be activated?
        if (knobLever.value <= activationThreshold)
        {
            StartCoroutine(SnapToValue(0f, true));
        }
        // else reset it
        else
        {
            StartCoroutine(SnapToValue(1f, false));
        }
    }

    private void CompleteActivation()
    {
        if (isLocked) return;
        isLocked = true;

        Debug.Log("¡Palanca Activada y Bloqueada!");

        audioSource.Stop();
        audioSource.PlayOneShot(activateSFX);

        OnLeverActivated.Invoke();

        knobLever.enabled = false;
    }

    // Corrutina para mover la palanca suavemente (animación de Snap)
    private IEnumerator SnapToValue(float targetValue, bool activateAfter)
    {
        float startValue = knobLever.value;
        float elapsed = 0f;

        // Desactivamos el script mientras se mueve sola para que nadie la toque
        knobLever.enabled = false;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / snapDuration;
            
            // Interpolación suave (SmoothStep)
            t = t * t * (3f - 2f * t);

            knobLever.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        knobLever.value = targetValue;

        if (activateAfter)
        {
            CompleteActivation();
            // Nota: No reactivamos knobLever.enabled porque CompleteActivation lo deja en false
        }
        else
        {
            // Si era un reset (volver a 0), reactivamos el script para que el jugador pueda volver a intentarlo
            knobLever.enabled = true;
        }
    }
}