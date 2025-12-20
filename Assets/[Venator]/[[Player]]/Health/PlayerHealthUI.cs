using UnityEngine;
using System.Collections;

/// <summary>
/// UI component that listens to PlayerHealth state changes and updates the visual display.
/// Specifically depends on PlayerHealth for HealthState information.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Blood Damage Panel")]
    [SerializeField] private GameObject damagePanel;
    [SerializeField] private float bloodPulseSpeed = 3f;

    private CanvasGroup damagePanelCG;
    private Coroutine bloodPulseCoroutine;

    private void Awake()
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealthUI: playerHealth is not assigned!");
        }

        // Setup damage panel
        if (damagePanel != null)
        {
            damagePanelCG = damagePanel.GetComponent<CanvasGroup>();
            if (damagePanelCG == null) damagePanelCG = damagePanel.AddComponent<CanvasGroup>();

            damagePanelCG.alpha = 0;
            damagePanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (playerHealth == null) return;

        playerHealth.OnStateChanged += UpdateUI;
        UpdateUI(playerHealth.State);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnStateChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Updates the UI based on the current health state.
    /// </summary>
    /// <param name="state">The current health state.</param>
    private void UpdateUI(HealthState state)
    {
        UpdateDamagePanel(state);
        Debug.Log($"PlayerHealthUI: Updated UI to {state}");
    }

    /// <summary>
    /// Updates the damage panel based on the current health state.
    /// </summary>
    /// <param name="state">The current health state.</param>
    private void UpdateDamagePanel(HealthState state)
    {
        if (damagePanel == null) return;

        // Stop any existing blood pulse effect
        if (bloodPulseCoroutine != null)
        {
            StopCoroutine(bloodPulseCoroutine);
            bloodPulseCoroutine = null;
        }

        switch (state)
        {
            case HealthState.Healthy:
                // No UI changes for healthy state - deactivate panel
                damagePanel.SetActive(false);
                if (damagePanelCG != null) damagePanelCG.alpha = 0;
                break;

            case HealthState.Hurt:
            case HealthState.Critical:
            case HealthState.Dead:
                // Use damage panel the same way for all damaged states
                damagePanel.SetActive(true);
                bloodPulseCoroutine = StartCoroutine(BloodPulseEffect());
                break;
        }
    }

    /// <summary>
    /// Coroutine that creates a pulsing blood effect for damaged states.
    /// </summary>
    private IEnumerator BloodPulseEffect()
    {
        while (damagePanelCG != null && playerHealth != null && playerHealth.State != HealthState.Healthy)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * bloodPulseSpeed));
            float alphaAjustado = 0.2f + (alpha * 0.5f);
            damagePanelCG.alpha = alphaAjustado;
            yield return null;
        }
    }
}
