using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public int slotID; // 0: Left, 1: Right, 2: Down
    public Image targetImage; // Arrastra la imagen aquí en el Inspector

    private InventoryTouchManager manager;
    private Color originalColor;
    public Color hoverColor = Color.green;
    private bool isSetup = false;

    // Esta función la llamará el Manager automáticamente
    public void Setup(InventoryTouchManager newManager)
    {
        manager = newManager;

        // Auto-detectar imagen si se te olvidó ponerla en el inspector
        if (targetImage == null) targetImage = GetComponent<Image>();

        if (targetImage != null)
        {
            originalColor = targetImage.color;
            isSetup = true;
        }
        else
        {
            Debug.LogError($"[InventorySlot] Error: El slot {gameObject.name} no tiene Imagen asignada.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSetup) return;

        // Verificamos si es el cursor de la mano
        if (other.CompareTag("Player") || other.name.Contains("Cursor"))
        {
            if (targetImage != null) targetImage.color = hoverColor;
            if (manager != null) manager.SetCurrentSelection(slotID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isSetup) return;

        if (other.CompareTag("Player") || other.name.Contains("Cursor"))
        {
            ResetColor();
            if (manager != null) manager.ClearSelection(slotID);
        }
    }

    public void ResetColor()
    {
        if (targetImage != null) targetImage.color = originalColor;
    }
}