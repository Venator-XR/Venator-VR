using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public int slotID;
    public Image targetImage;
    public TextMeshProUGUI labelText;

    private InventoryTouchManager manager;
    private Color originalColor;

    // Color al que cambia cuando se selecciona
    public Color hoverColor = Color.green;

    public void Setup(InventoryTouchManager newManager)
    {
        manager = newManager;
        if (targetImage == null) targetImage = GetComponent<Image>();

        if (targetImage != null)
        {
            // Guardamos el color original (blanco/gris) para volver a él luego
            originalColor = targetImage.color;
        }
    }

    public void UpdateLabel(string text)
    {
        if (labelText != null) labelText.text = text;
    }

    // --- FUNCIONES VISUALES (Controladas por el Manager) ---

    public void Highlight()
    {
        if (targetImage != null) targetImage.color = hoverColor;
    }

    public void ResetColor()
    {
        if (targetImage != null) targetImage.color = originalColor;
    }

    // --- DETECCIÓN FÍSICA ---

    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionamos a la mano o cursor
        if (other.CompareTag("Player") || other.name.Contains("Cursor"))
        {
            // AVISAMOS AL JEFE: "Soy el ID tal y me han tocado"
            if (manager != null) manager.OnSlotHovered(slotID);
        }
    }

    // Ya no usamos OnTriggerExit para limpiar, lo gestiona el manager
}