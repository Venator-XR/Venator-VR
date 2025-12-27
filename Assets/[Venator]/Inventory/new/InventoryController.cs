using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject uiCanvas;
    public Transform headCamera;
    public Transform rightHandPosition;
    public HandEquipmentManager handManager;

    [Header("Data")]
    public InventoryItemData pistolData;
    public InventoryItemData currentPocketItem;

    [Header("Input")]
    public InputActionProperty menuButton;

    [Header("Slots UI")]
    public InventorySlot slotPistol;
    public InventorySlot slotPocket;
    public InventorySlot slotHand;

    private InventorySlot currentHoveredSlot;

    void Start()
    {
        uiCanvas.SetActive(false);

        if (menuButton.action != null) 
        menuButton.action.Enable();

        // Setup slots
        slotPistol.Setup(this);
        slotPocket.Setup(this);
        slotHand.Setup(this);
        
        UpdatePocketUI();
    }

    void Update()
    {
        if (menuButton != null && menuButton.action.WasPressedThisFrame())
        {
            OpenInventory();
        }

        if (menuButton != null && menuButton.action.WasReleasedThisFrame())
        {
            ExecuteAction();
            CloseInventory();
        }
    }

    void OpenInventory()
    {
        Debug.Log("Inventory opened");
        uiCanvas.SetActive(true);

        // Position in front of the hand
        uiCanvas.transform.position = rightHandPosition.position + (Vector3.up * 0.1f) + (Vector3.forward * -0.05f);

        Vector3 directionToHead = headCamera.position - uiCanvas.transform.position;
        directionToHead.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToHead);
        Quaternion tiltUp = Quaternion.Euler(-20f, 0f, 0f); // Adjust -30f for desired tilt angle
        uiCanvas.transform.rotation = lookRotation * tiltUp;
    }

    void CloseInventory()
    {
        Debug.Log("Inventory closed");
        uiCanvas.SetActive(false);
        currentHoveredSlot = null;
    }

    public void OnSlotHover(InventorySlot slot) => currentHoveredSlot = slot;
    public void OnSlotExit(InventorySlot slot) { if (currentHoveredSlot == slot) currentHoveredSlot = null; }

    void ExecuteAction()
    {
        if (currentHoveredSlot == null) return;

        switch (currentHoveredSlot.type)
        {
            case InventorySlot.SlotType.Pistol:
                // Equip Pistol
                handManager.EquipItem(pistolData);
                break;

            case InventorySlot.SlotType.Pocket:
                HandlePocketInteraction();
                break;

            case InventorySlot.SlotType.Hand: // NUEVO CASO
                UnequipAndHolster();
                break;
        }
    }

    void HandlePocketInteraction()
    {
        // Swap Logic

        InventoryItemData heldItem = handManager.GetHeldItemData();
        InventoryItemData pocketItem = currentPocketItem;

        // if pocket had something
        if (heldItem != null)
        {
            // Drop old item
            if (pocketItem != null)
            {
                Instantiate(pocketItem.modelPrefab, rightHandPosition.position, Quaternion.identity);
            }

            // Save new
            currentPocketItem = heldItem;

            // erase from hand
            handManager.UnequipCurrent();
        }
        else
        {
            // No item held
            if (pocketItem != null)
            {
                // Equip
                handManager.EquipItem(pocketItem);

                // Erase from inventory
                currentPocketItem = null;
            }
        }

        UpdatePocketUI();
    }

    void UnequipAndHolster()
    {
        // Check held item
        InventoryItemData heldItem = handManager.GetHeldItemData();

        if (heldItem != null)
        {
            if (heldItem == pistolData)
            {
                // pistol always stores itself
                handManager.UnequipCurrent();
            }
            else
            {
                // if pocket is empty   
                if (currentPocketItem == null)
                {
                    // store item
                    currentPocketItem = heldItem;
                    UpdatePocketUI();

                    // erase from hand
                    handManager.UnequipCurrent();
                }
                else
                {
                    // if pocket has something we drop it
                    handManager.DropToWorld();
                }
            }
        }
    }

    void UpdatePocketUI()
    {
        if (currentPocketItem != null) slotPocket.SetIcon(currentPocketItem.icon);
        else slotPocket.SetIcon(null);
    }
}