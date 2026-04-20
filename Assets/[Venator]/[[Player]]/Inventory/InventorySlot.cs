using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public enum SlotType { Pistol, Pocket, Hand }
    public SlotType type;

    [Header("Visuals")]
    public Image iconImage;
    public Color normalColor = new Color(0, 0, 0, 0.8f);
    public Color highlightColor = new Color(75, 0, 0, 0.8f);
    public Image hoverImage;
    
    private InventoryController controller;

    public void Setup(InventoryController ctrl) => controller = ctrl;

    // Detect collision with PlayerHand
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            if (hoverImage) hoverImage.color = highlightColor;
            controller.OnSlotHover(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.CompareTag("PlayerHand"))
        {
            ResetSlot();
        }
    }

    public void OnSlotExit(Collider other = null)
    {
        ResetSlot();
    }

    private void ResetSlot()
    {
        if (hoverImage != null) hoverImage.color = normalColor;
        if (controller != null) controller.OnSlotExit(this);
    }

    // Update pocket icon
    public void SetIcon(Sprite s)
    {
        if (iconImage)
        {
            iconImage.sprite = s;
            iconImage.enabled = (s != null);
        }
    }
}