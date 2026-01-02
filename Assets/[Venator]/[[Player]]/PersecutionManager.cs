using System.Collections;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages persecution mode damage interactions with enemies.
/// Uses the existing PlayerHealth system to apply damage.
/// </summary>
public class PersecutionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Persecution Configuration")]
    [SerializeField] private int persecutionDamage = 2;
    [SerializeField] private string defeatSceneName = "Pantalla_Derrota";
    [SerializeField] private float vampireSlowdownDuration = 10f;
    [SerializeField] float cooldownTime = 3f;
    private bool cooldown = false;

    private void Awake()
    {
        // Try to find PlayerHealth if not assigned
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PersecutionManager: PlayerHealth component not found! Please assign it in the inspector.");
            }
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && playerHealth != null)
        {
            if (!cooldown)
            {
                Debug.Log("Trigger enter, applying dmg");
                ApplyEnemyDamage();
            }
        }
    }

    /// <summary>
    /// Applies persecution damage when hit by an enemy.
    /// </summary>
    private void ApplyEnemyDamage()
    {
        cooldown = true;
        StartCoroutine(Cooldown(cooldownTime));
        playerHealth.ApplyDamage(persecutionDamage);

        // Slow down the vampire on hit
        FollowPlayerAgent scriptVampiro = FindFirstObjectByType<FollowPlayerAgent>();
        if (scriptVampiro == null) Debug.Log("followPlayerAgent not found");
        else scriptVampiro.StunVampire(vampireSlowdownDuration);
    }

    private IEnumerator Cooldown(float wait)
    {
        yield return new WaitForSeconds(wait);
        cooldown = false;
    }

    /// <summary>
    /// Handles player death by loading the defeat scene.
    /// </summary>
    private void HandlePlayerDeath()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(defeatSceneName);
    }
}