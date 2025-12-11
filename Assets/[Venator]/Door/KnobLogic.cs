using UnityEngine;

public class KnobLogic : MonoBehaviour
{
    public Transform objetoParaMover; // Tu puerta

    public void OnKnobChange(float valor)
    {
        // Debug para ver si funciona
        Debug.Log($"Valor: {valor}");

        if (objetoParaMover != null)
        {
            // CAMBIO: En vez de mover posición, rotamos en el eje Y (vertical)
            // Esto hará que la puerta gire de 0 a 90 grados según gires el pomo
            float anguloPuerta = valor * 90.0f; 
            
            // Aplicamos la rotación (asumiendo que la puerta cerrada es rotación 0,0,0)
            objetoParaMover.localRotation = Quaternion.Euler(0, anguloPuerta, 0);
        }
    }
}