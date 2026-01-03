using UnityEngine;
using UnityEngine.InputSystem;
using System;

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
    public Sprite emptyIcon;

    private InventorySlot currentHoveredSlot;

    public event Action OnInventoryOpened;
    public event Action OnInventoryClosed;

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
        InventoryItemData heldItem = handManager.GetHeldItemData();
        uiCanvas.SetActive(true);

        // Position in front of the hand
        uiCanvas.transform.position = rightHandPosition.position + (Vector3.up * 0.05f) + (Vector3.forward * 0f);

        Vector3 directionToHead = headCamera.position - uiCanvas.transform.position;
        directionToHead.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToHead);
        Quaternion tiltUp = Quaternion.Euler(-20f, 0f, 0f);
        uiCanvas.transform.rotation = lookRotation * tiltUp;

        OnInventoryOpened?.Invoke();
    }

    void CloseInventory()
    {
        Debug.Log("Inventory closed");

        uiCanvas.SetActive(false);

        if (currentHoveredSlot != null)
        {
            currentHoveredSlot.OnSlotExit(null);
        }

        currentHoveredSlot = null;

        OnInventoryClosed?.Invoke();
    }

    public void OnSlotHover(InventorySlot slot) => currentHoveredSlot = slot;
    public void OnSlotExit(InventorySlot slot) { if (currentHoveredSlot == slot) currentHoveredSlot = null; }

    void ExecuteAction()
{
    if (currentHoveredSlot == null) return;

    switch (currentHoveredSlot.type)
    {
        case InventorySlot.SlotType.Pistol:
            InventoryItemData heldItem = handManager.GetHeldItemData();
            
            // heldItem is NOT pistol and pocket empty, store item
            if (heldItem != null && heldItem != pistolData)
            {
                if (currentPocketItem == null)
                {
                    currentPocketItem = heldItem;
                    UpdatePocketUI();
                }
                else
                {
                    // pocket had something, drop it
                    handManager.DropToWorld(); 
                }
            }
            // ---------------------------------------

            handManager.EquipItem(pistolData);
            break;

        case InventorySlot.SlotType.Pocket:
            HandlePocketInteraction();
            break;

        case InventorySlot.SlotType.Hand:
            UnequipAndHolster();
            break;
    }
}

    void HandlePocketInteraction()
    {
        InventoryItemData heldItem = handManager.GetHeldItemData();
        InventoryItemData pocketItem = currentPocketItem;

        // Pistol held
        if (heldItem != null && heldItem == pistolData)
        {
            // unequip pistol
            handManager.UnequipCurrent();

            // if pocket had something equip it
            if (pocketItem != null)
            {
                handManager.EquipItem(pocketItem);
                currentPocketItem = null;
            }
        }

        // item held (not pistol)
        else if (heldItem != null)
        {
            // if pocket had item, drop it
            if (pocketItem != null)
            {
                Vector3 spawnPos = rightHandPosition.position + (headCamera.forward * 0.2f);
                Instantiate(pocketItem.modelPrefab, spawnPos, Quaternion.identity);
            }

            // held item now is the new pocket item
            currentPocketItem = heldItem;
            handManager.UnequipCurrent();
        }
        // empty hand
        else
        {
            if (pocketItem != null)
            {
                handManager.EquipItem(pocketItem);
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
                Debug.Log("heldItem == pistol");
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
        else slotPocket.SetIcon(emptyIcon);
    }
}