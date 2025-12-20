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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemigo") && playerHealth != null)
        {
            ApplyEnemyDamage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") && playerHealth != null)
        {
            ApplyEnemyDamage();
        }
    }

    /// <summary>
    /// Applies persecution damage when hit by an enemy.
    /// </summary>
    private void ApplyEnemyDamage()
    {
        playerHealth.ApplyDamage(persecutionDamage);

        // Slow down the vampire on hit
        Unity.AI.Navigation.Samples.FollowPlayerAgent scriptVampiro = FindFirstObjectByType<Unity.AI.Navigation.Samples.FollowPlayerAgent>();
        if (scriptVampiro != null)
        {
            scriptVampiro.RalentizarVampiro(vampireSlowdownDuration);
        }
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