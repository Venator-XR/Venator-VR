# Escena: Campo de Tiro (Shooting Range)
Documento con mecánicas y sistemas de pruebas de armas y linterna VR.

## Resumen de Sistemas

### 1. Sistema de Linterna (`FlashlightController`)
- Encendido: vía botón (`Trigger` / `Activate`).
- Mecánica "Shake": si está apagada y el jugador la agita con fuerza (velocidad de la punta), se enciende automáticamente (estilo dinamo).
- API pública: métodos `TurnOn()`, `TurnOff()` y propiedad `IsOn` para integrar con batería, triggers de miedo, etc.

### 2. Sistema de Armas (`PistolaManager` & `LaserProjectile`)
- Pistola de energía que dispara proyectiles físicos sin caída (láseres).
- Auto-ignorar colisión: detecta el `Collider` del arma y evita que la bala choque con el cañón al salir.
- Visuales independientes: proyectil con jerarquía Padre (Físicas) / Hijo (Visuales rotados 90°) para evitar problemas de orientación.
- Configuración: `Velocidad`, `Daño` y `Tipo de Daño` (string) configurables desde el inspector de `PistolaManager`.

## 💥 Sistema de Daño
Sistema desacoplado basado en interfaces: el arma no sabe qué golpea; entrega un paquete `DamageInfo`.
- `IDamageable` (interfaz): contrato para cualquier objeto destructible.
- `DamageInfo` (struct): contiene `amount` (float) y `dataType` (string, ej. "Plasma").

## 📋 Guía: hacer que un modelo reciba daño

### Paso 1: Requisitos físicos
- El objeto (o uno de sus hijos) debe tener un `Collider` (`BoxCollider`, `MeshCollider`, etc.).
- Nota: no es necesario `Rigidbody` si es un objeto estático (pared, diana fija).

### Paso 2: El script
- Crea un script nuevo (p. ej., `EnemigoRobot.cs`) y pégalo en el objeto.
- Debe heredar de `MonoBehaviour` e implementar la interfaz `IDamageable`.

### Paso 3: Plantilla de código (copy-paste)

```csharp
using UnityEngine;

// 1. Añade ", IDamageable" después de MonoBehaviour
public class MiNuevoObjeto : MonoBehaviour, IDamageable
{
    [SerializeField] private float vida = 100f;

    // 2. Implementa OBLIGATORIAMENTE esta función
    public void TakeDamage(DamageInfo info)
    {
        // Lógica de recibir daño
        vida -= info.amount;
        Debug.Log($"Me dieron con {info.dataType}! Vida restante: {vida}");

        // Ejemplo: Reacción visual
        GetComponent<Renderer>().material.color = Color.red;

        // Ejemplo: Muerte
        if (vida <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Lógica de destrucción, partículas, sonido, etc.
        Destroy(gameObject);
    }
}
```

### Paso 4: Diagnóstico de errores comunes
- ¿Tiene `Collider`? Sin collider, la bala atraviesa el modelo.
- ¿Está el script en el padre o hijo? El proyectil busca `GetComponentInParent<IDamageable>`. Debe estar en el objeto golpeado o en cualquiera de sus padres (no en un hijo).
- ¿El collider es `IsTrigger`? Si es trigger, la bala física podría no detectarlo con `OnCollisionEnter`. Asegúrate de que sea sólido o revisa que el proyectil maneje triggers.

## ⚠️ Notas técnicas para desarrolladores
- Proyectiles flotantes: en el prefab de la bala, `Rigidbody` con `Use Gravity: false` y `Is Kinematic: false`.
- Orientación de balas: no rotes el objeto raíz del proyectil por código. Rota el hijo `Visuals` dentro del prefab si el modelo 3D sale mal orientado.
- Colisiones: no cambies la `Layer` de la pistola para evitar colisiones con la bala (rompe XR Grab). `LaserProjectile` gestiona `Physics.IgnoreCollision` en `Initialize()`.