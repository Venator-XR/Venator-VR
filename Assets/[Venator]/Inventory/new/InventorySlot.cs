using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public enum SlotType { Pistol, Pocket, Hand }
    public SlotType type;

    [Header("Visuals")]
    public Image iconImage;
    public Color normalColor = new Color(1, 1, 1, 0.5f);
    public Color highlightColor = Color.yellow;

    private Image bgImage;
    private InventoryController controller;

    void Awake()
    {
        bgImage = GetComponent<Image>();
        if (bgImage) bgImage.color = normalColor;
    }

    public void Setup(InventoryController ctrl) => controller = ctrl;

    // Detect collision with PlayerHand
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("Collision with hand");
            if (bgImage) bgImage.color = highlightColor;
            controller.OnSlotHover(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            if (bgImage) bgImage.color = normalColor;
            controller.OnSlotExit(this);
        }
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