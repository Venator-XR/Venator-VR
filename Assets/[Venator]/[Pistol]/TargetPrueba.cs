using UnityEngine;

// ¡Importante! Heredamos de MonoBehaviour Y firmamos el contrato IDamageable
public class TargetPrueba : MonoBehaviour, IDamageable
{
    public float salud = 100f;

    // Estamos OBLIGADOS a implementar esta función por la interfaz
    public void TakeDamage(DamageInfo info)
    {
        salud -= info.amount;
        Debug.Log($"¡Auch! He recibido {info.amount} de daño. Tipo: {info.dataType}. Salud restante: {salud}");

        if (salud <= 0)
        {
            Debug.Log("¡Destruido!");
            Destroy(gameObject);
        }

        // Aquí podrías cambiar el color del cubo para dar feedback visual
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ResetColor", 0.2f);
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}