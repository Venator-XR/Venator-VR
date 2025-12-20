// IDamageable.cs
using UnityEngine;

// 1. Definimos el paquete de información que viajará con la bala
public struct DamageInfo
{
    public float amount;     // La cantidad de daño
    public string dataType;  // El "string de datos" extra que pediste (ej: "Fuego", "Hielo")
    // Podrías añadir aquí quién disparó, la posición del impacto, etc.
}

// 2. La interfaz: el contrato que dice "Puedo recibir daño"
public interface IDamageable
{
    // Cualquier script que use esta interfaz ESTÁ OBLIGADO a tener esta función
    void TakeDamage(DamageInfo info);
}