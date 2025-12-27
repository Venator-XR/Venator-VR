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
        InventoryItemData heldItem = handManager.GetHeldItemData();
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

        // IMPORTANTE: Si el slot se quedó iluminado, lo forzamos a apagarse
        if (currentHoveredSlot != null)
        {
            currentHoveredSlot.OnSlotExit(null); // Esto forzará al slot a resetear su color
        }

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
            // --- NUEVA LÓGICA DE AUTO-GUARDADO ---
            InventoryItemData heldItem = handManager.GetHeldItemData();
            
            // Si tengo algo que NO es la pistola y el bolsillo está libre, lo guardamos
            if (heldItem != null && heldItem != pistolData)
            {
                if (currentPocketItem == null)
                {
                    currentPocketItem = heldItem;
                    UpdatePocketUI();
                    // No hace falta llamar a UnequipCurrent aquí porque EquipItem lo hará luego
                }
                else
                {
                    // Si el bolsillo está ocupado, lo soltamos al suelo para no borrarlo
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

        // Caso 1: Tengo la PISTOLA en la mano
        if (heldItem != null && heldItem == pistolData)
        {
            // Simplemente la guardamos (desaparece)
            handManager.UnequipCurrent();

            // Y si el bolsillo tenía algo, lo sacamos a la mano
            if (pocketItem != null)
            {
                handManager.EquipItem(pocketItem);
                currentPocketItem = null;
            }
        }
        // Caso 2: Tengo un OBJETO COMÚN (llave, etc.) en la mano
        else if (heldItem != null)
        {
            // Si el bolsillo ya tenía algo, ese objeto viejo se cae al suelo (Swap)
            if (pocketItem != null)
            {
                Vector3 spawnPos = rightHandPosition.position + (headCamera.forward * 0.2f);
                Instantiate(pocketItem.modelPrefab, spawnPos, Quaternion.identity);
            }

            // El objeto que tenía en la mano pasa a ser el nuevo objeto del bolsillo
            currentPocketItem = heldItem;
            handManager.UnequipCurrent();
        }
        // Caso 3: Mano VACÍA
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
        else slotPocket.SetIcon(null);
    }
}