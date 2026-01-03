using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandEquipmentManager : MonoBehaviour
{
    [Header("Dependencies")]
    public XRBaseInteractor handInteractor;
    public Transform itemSpawnPoint;

    [Header("Visuals")]
    public GameObject realHandModel;

    private GameObject currentModel;
    private bool isLockedItem = false;
    private GameObject currentEquippedObject;

    bool isUnequipping = false;

    void OnEnable()
    {
        // Nos suscribimos a los eventos de la mano
        handInteractor.selectEntered.AddListener(OnObjectGrabbed);
        handInteractor.selectExited.AddListener(OnObjectReleased);
    }

    void OnDisable()
    {
        // Nos desuscribimos al apagar para evitar errores
        handInteractor.selectEntered.RemoveListener(OnObjectGrabbed);
        handInteractor.selectExited.RemoveListener(OnObjectReleased);
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        // ¡Aquí está la clave! 
        // Cuando coges algo (del suelo o del inventario), guardamos la referencia
        currentEquippedObject = args.interactableObject.transform.gameObject;
        if (realHandModel != null) realHandModel.SetActive(false);
        Debug.Log("Objeto detectado en mano: " + currentEquippedObject.name);
    }

    private void OnObjectReleased(SelectExitEventArgs args)
{
    // FIX: If we are running the Unequip function, stop here. 
    // Do not run drop logic, do not re-grab, do not pass go.
    if (isUnequipping) return;

    GameObject releasedObj = args.interactableObject.transform.gameObject;
    var refData = releasedObj.GetComponentInChildren<ItemReference>();

    // Always restore the hand first
    if (realHandModel != null)
        realHandModel.SetActive(true);

    // pistol not droppable
    if (refData != null && refData.data != null && refData.data.isDroppable == false)
    {
        // block manual drop by reselecting it
        IXRSelectInteractable interactable = releasedObj.GetComponent<IXRSelectInteractable>();
        // Check if interactable is still valid before re-selecting
        if (interactable != null) 
        {
             handInteractor.interactionManager.SelectEnter(handInteractor, interactable);
        }
        return;
    }

    if (currentEquippedObject == releasedObj)
    {
        currentEquippedObject = null;
    }
}



    public void EquipItem(InventoryItemData data)
    {
        if (data == null || data.modelPrefab == null) return;

        UnequipCurrent();

        Debug.Log("EquipItem(" + data + ")");
        // Ahora currentEquippedObject ya existe y puede guardar la referencia
        GameObject newObject = Instantiate(data.modelPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
        currentEquippedObject = newObject;

        IXRSelectInteractable interactable = newObject.GetComponentInChildren<IXRSelectInteractable>();

        if (interactable != null)
        {
            Debug.Log("Found! forcing select on " + ((MonoBehaviour)interactable).name);
            handInteractor.interactionManager.SelectEnter(handInteractor, interactable);
        }
        else
        {
            Debug.LogError("ERROR: prefab " + data.modelPrefab.name + " doesnt have XR Grab / Locked Interactable");
        }
    }

    public void UnequipCurrent()
{
    if (currentEquippedObject != null)
    {
        GameObject tempObj = currentEquippedObject;
        Debug.Log("Forzando desequipado de: " + tempObj.name);

        if (handInteractor.hasSelection)
        {
            // 1. Raise the flag so OnObjectReleased ignores this event
            isUnequipping = true; 

            var selected = handInteractor.interactablesSelected[0];
            handInteractor.interactionManager.SelectExit(handInteractor, selected);
            
            // 2. Lower the flag immediately after the event fires
            isUnequipping = false; 
        }

        // 3. Clear the reference
        currentEquippedObject = null;

        // 4. IMPORTANT: Since we skipped OnObjectReleased, we must manually show the hand here
        if (realHandModel != null)
        {
            realHandModel.SetActive(true);
        }

        // 5. Now it is safe to destroy
        Destroy(tempObj);

        Debug.Log("Objeto destruido con éxito.");
    }
}

    // Actual drop of item
    public void DropToWorld()
    {
        Debug.Log("DropToWorld()");

        if (currentModel == null || isLockedItem) return; // Return if locked item (cant drop it)

        // Force select exit
        handInteractor.interactionManager.SelectExit((IXRSelectInteractor)handInteractor, currentModel.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>());

        currentModel.transform.SetParent(null);
        currentModel = null;
    }

    public bool IsHoldingItem() => handInteractor.hasSelection;

    //-------------------------
    public InventoryItemData GetHeldItemData()
    {
        if (handInteractor.interactablesSelected.Count == 0)
        {
            Debug.Log("GetHeldItemData: No hay nada seleccionado en el Interactor.");
            return null;
        }

        var selected = handInteractor.interactablesSelected[0];

        if (selected == null || selected.transform == null)
        {
            Debug.Log("Held reference dead, cleaning");
            return null;
        }

        GameObject obj = selected.transform.gameObject;


        var refScript = obj.GetComponentInChildren<ItemReference>();

        if (refScript != null)
        {
            Debug.Log("GetHeldItemData: Detectado item -> " + refScript.data.ID);
            return refScript.data;
        }
        else
        {
            Debug.LogWarning("GetHeldItemData: El objeto " + obj.name + " no tiene ItemReference en raíz ni hijos.");
            return null;
        }
    }
}