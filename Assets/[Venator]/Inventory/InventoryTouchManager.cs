using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InventoryTouchManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Transform spawnPoint;

    [Header("Referencias a los Slots")]
    [SerializeField] private InventorySlot slotScriptLeft;
    [SerializeField] private InventorySlot slotScriptRight;
    [SerializeField] private InventorySlot slotScriptDown;
    [SerializeField] private InventorySlot slotScriptDrop;

    [Header("Sistema de Mano")]
    [SerializeField] private XRInteractionGroup handGroup;
    [SerializeField] private XRBaseInteractor mainInteractor;

    [Header("Modelos Equipados")]
    [SerializeField] private GameObject modelPistol;
    [SerializeField] private GameObject modelHand;

    private GameObject currentLeftItem = null;

    [Header("Input")]
    [SerializeField] private InputActionProperty toggleButton;
    [SerializeField] private InputActionProperty activateButton;

    private bool isInventoryOpen = false;
    private int currentSelectedID = -1;

    void Start()
    {
        if (toggleButton.action != null) toggleButton.action.Enable();
        if (activateButton.action != null) activateButton.action.Enable();

        if (slotScriptLeft) slotScriptLeft.Setup(this);
        if (slotScriptRight) slotScriptRight.Setup(this);
        if (slotScriptDown) slotScriptDown.Setup(this);
        if (slotScriptDrop) slotScriptDrop.Setup(this);

        uiCanvas.SetActive(false);
        UpdateLeftSlotText("Vacío");
        EquipHand();
    }

    void Update()
    {
        // 1. ABRIR / CERRAR
        if (toggleButton.action.WasPressedThisFrame())
        {
            if (CanOpenInventory()) OpenInventory();
            else Debug.Log("Inventario bloqueado.");
        }

        if (toggleButton.action.WasReleasedThisFrame()) CloseAndEquip();

        // 2. USAR ITEM (LINTERNA)
        // Solo funciona si el inventario está CERRADO
        if (!isInventoryOpen && currentLeftItem != null && activateButton.action.WasPressedThisFrame())
        {
            var handler = currentLeftItem.GetComponent<ItemActionHandler>();
            if (handler != null)
            {
                handler.OnAction();
            }
            else
            {
                Debug.LogWarning($"[ITEM] El objeto {currentLeftItem.name} no tiene el script 'ItemActionHandler'.");
            }
        }
    }

    public void SetCurrentSelection(int id) => currentSelectedID = id;

    public void ClearSelection(int id)
    {
        // Descomenta esto si quieres que al salir del botón se deseleccione inmediatamente
        // if (currentSelectedID == id) currentSelectedID = -1;
    }

    public void OnSlotHovered(int id)
    {
        currentSelectedID = id;
        UpdateVisuals(); // Actualizamos todos los colores a la vez
    }

    private void UpdateVisuals()
    {
        // 1. Reseteamos TODOS a su color original
        if (slotScriptLeft) slotScriptLeft.ResetColor();
        if (slotScriptRight) slotScriptRight.ResetColor();
        if (slotScriptDown) slotScriptDown.ResetColor();
        if (slotScriptDrop) slotScriptDrop.ResetColor();

        // 2. Iluminamos SOLO el seleccionado
        switch (currentSelectedID)
        {
            case 0: if (slotScriptLeft) slotScriptLeft.Highlight(); break;
            case 1: if (slotScriptRight) slotScriptRight.Highlight(); break;
            case 2: if (slotScriptDown) slotScriptDown.Highlight(); break;
            case 3: if (slotScriptDrop) slotScriptDrop.Highlight(); break;
        }
    }
    private void OpenInventory()
    {
        isInventoryOpen = true;
        uiCanvas.transform.position = spawnPoint.position;
        uiCanvas.transform.LookAt(Camera.main.transform);
        uiCanvas.SetActive(true);

        // Al abrir, reseteamos visuales por si acaso
        currentSelectedID = -1;
        UpdateVisuals();
    }

    private void CloseAndEquip()
    {
        if (!isInventoryOpen) return;

        isInventoryOpen = false;
        uiCanvas.SetActive(false);

        switch (currentSelectedID)
        {
            case 0: EquipLeft(); break;
            case 1: EquipPistol(); break;
            case 2: EquipHand(); break;
            case 3: DropCurrentItem(); break;
        }

        currentSelectedID = -1;
        UpdateVisuals(); // Limpiamos colores
    }

    private void EquipLeft()
    {
        if (mainInteractor != null && mainInteractor.hasSelection)
        {
            IXRSelectInteractable interactable = mainInteractor.interactablesSelected[0];
            GameObject objGrabbable = interactable.transform.gameObject;

            if (objGrabbable.CompareTag("Item")) StoreObjectAsItem(objGrabbable);
        }
        else if (currentLeftItem != null)
        {
            ToggleGrab(false);
            DisableAllModels();
            if (modelHand) modelHand.SetActive(false);
            currentLeftItem.SetActive(true);
        }
        else
        {
            EquipHand();
        }
    }

    private void StoreObjectAsItem(GameObject obj)
    {
        ForceReleaseObject();

        if (currentLeftItem != null && currentLeftItem != obj) DropCurrentItem();

        currentLeftItem = obj;
        UpdateLeftSlotText(obj.name);

        // Dormir Físicas
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        var grab = obj.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        var col = obj.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        EquipLeft();
    }

    public void DropCurrentItem()
    {
        if (currentLeftItem == null)
        {
            Debug.Log("[DROP] No hay nada que tirar.");
            return;
        }

        GameObject itemToDrop = currentLeftItem;
        currentLeftItem = null;
        UpdateLeftSlotText("Vacío");

        // 1. Desvincular
        itemToDrop.transform.SetParent(null);
        itemToDrop.SetActive(true);

        // 2. Restaurar Físicas
        var col = itemToDrop.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        var rb = itemToDrop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            // EMPUJÓN: Lo lanzamos un poco hacia adelante para que no caiga recto
            rb.AddForce(this.transform.forward * 2f, ForceMode.Impulse);
        }

        var grab = itemToDrop.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = true;

        Debug.Log($"[DROP] Objeto {itemToDrop.name} tirado al suelo.");

        // Volvemos a la mano normal
        EquipHand();
    }

    private void UpdateLeftSlotText(string text)
    {
        if (slotScriptLeft != null) slotScriptLeft.UpdateLabel(text);
    }

    private void DisableAllModels()
    {
        if (modelPistol) modelPistol.SetActive(false);
        if (modelHand) modelHand.SetActive(true);
        if (currentLeftItem != null) currentLeftItem.SetActive(false);
    }

    private void EquipPistol()
    {
        ToggleGrab(false);
        DisableAllModels();
        if (modelHand) modelHand.SetActive(false);
        if (modelPistol) modelPistol.SetActive(true);
    }

    private void EquipHand()
    {
        DisableAllModels();
        ToggleGrab(true);
    }

    private bool CanOpenInventory()
    {
        if (isInventoryOpen) return true;
        if (mainInteractor == null || !mainInteractor.hasSelection) return true;
        IXRSelectInteractable obj = mainInteractor.interactablesSelected[0];
        if (obj.transform.CompareTag("Fixed")) return false;
        return true;
    }

    private void ForceReleaseObject()
    {
        if (mainInteractor != null && mainInteractor.hasSelection)
            mainInteractor.interactionManager.CancelInteractorSelection((IXRSelectInteractor)mainInteractor);
    }

    private void ToggleGrab(bool enable)
    {
        if (!enable) ForceReleaseObject();
        if (handGroup != null) handGroup.enabled = enable;
        if (mainInteractor != null) mainInteractor.enabled = enable;
    }
}