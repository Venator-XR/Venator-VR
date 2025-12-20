using UnityEngine;
using System;

public class TargetPrueba : MonoBehaviour, IHealth
{
    public float salud = 100f;
    private bool isVulnerable = true;

    /// <summary>
    /// Invoked when the target dies.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// Gets whether the target can currently take damage.
    /// </summary>
    public bool IsVulnerable => isVulnerable && salud > 0;

    // Implementamos ApplyDamage de la interfaz IHealth
    public void ApplyDamage(int amount)
    {
        if (!IsVulnerable) return;

        salud -= amount;
        Debug.Log($"�Auch! He recibido {amount} de da�o. Salud restante: {salud}");

        if (salud <= 0)
        {
            Kill();
        }
        else
        {
            // Aqu� podr�as cambiar el color del cubo para dar feedback visual
            GetComponent<Renderer>().material.color = Color.red;
            Invoke("ResetColor", 0.2f);
        }
    }

    /// <summary>
    /// Instantly kills the target.
    /// </summary>
    public void Kill()
    {
        Debug.Log("�Destruido!");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}