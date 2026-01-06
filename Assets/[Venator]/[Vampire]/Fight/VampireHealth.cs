using System;
using UnityEngine;

/// <summary>
/// Health implementation for the vampire boss.
/// </summary>
public class VampireHealth : MonoBehaviour, IHealth
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool startVulnerable = true;

    private int _currentHealth;
    private bool _isVulnerable;
    private bool _isDead;

    public event Action OnDeath;
    
    public event Action<int> OnHealthChanged;

    public bool IsVulnerable => _isVulnerable;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;

    /// <summary>
    /// Sets the vulnerability state of the vampire.
    /// </summary>
    public void SetVulnerability(bool vulnerable)
    {
        _isVulnerable = vulnerable;
    }

    private void Awake()
    {
        _currentHealth = maxHealth;
        _isVulnerable = startVulnerable;
        _isDead = false;
    }

    public void ApplyDamage(int amount)
    {
        if (_isDead || !_isVulnerable || amount <= 0)
            return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Max(0, _currentHealth);


        Debug.Log($"Vampire took {amount} damage. Health: {_currentHealth}/{maxHealth}");

        if (_currentHealth <= 0)
        {
            Kill();
        }
        
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void Kill()
    {
        if (_isDead)
            return;

        _isDead = true;
        _currentHealth = 0;
        _isVulnerable = false;

        Debug.Log("Vampire has been defeated!");
        OnDeath?.Invoke();
    }

    /// <summary>
    /// Heals the vampire by the specified amount.
    /// </summary>
    public void Heal(int amount)
    {
        if (_isDead || amount <= 0)
            return;

        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);

        Debug.Log($"Vampire healed {amount}. Health: {_currentHealth}/{maxHealth}");
    }
}