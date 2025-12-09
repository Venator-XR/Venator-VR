using UnityEngine;

/// <summary>
/// UI component that listens to IHealth state changes and updates the visual display.
/// Does not know about health numbers, only responds to HealthState changes.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject healthObject;
    [SerializeField] private SpriteRenderer renderer;

    [Header("Sprites")]
    [SerializeField] private Sprite healthySprite;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private Sprite criticalSprite;
    [SerializeField] private Sprite deadSprite;

    private IHealth health;

    private void Awake()
    {
        if (healthObject == null)
        {
            Debug.LogError("PlayerHealthUI: healthObject is not assigned!");
            return;
        }

        health = healthObject.GetComponent<IHealth>();
        if (health == null)
        {
            Debug.LogError($"PlayerHealthUI: No component implementing IHealth found on {healthObject.name}!");
        }
    }

    private void OnEnable()
    {
        if (health == null) return;

        health.OnStateChanged += UpdateUI;
        UpdateUI(health.State);
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnStateChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Updates the UI based on the current health state.
    /// </summary>
    /// <param name="state">The current health state.</param>
    private void UpdateUI(HealthState state)
    {
        if (renderer == null) return;

        switch (state)
        {
            case HealthState.Healthy:  renderer.sprite = healthySprite;  break;
            case HealthState.Hurt:     renderer.sprite = hurtSprite;     break;
            case HealthState.Critical: renderer.sprite = criticalSprite; break;
            case HealthState.Dead:     renderer.sprite = deadSprite;     break;
        }

        Debug.Log($"PlayerHealthUI: Updated UI to {state}");
    }
}
