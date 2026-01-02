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
        // Solo ponemos a null si el objeto que se suelta es el que tenemos registrado
        // Si ya es null porque UnequipCurrent lo está gestionando, no pasa nada.
        if (currentEquippedObject != null && currentEquippedObject == args.interactableObject.transform.gameObject)
        {
            currentEquippedObject = null;
            if (realHandModel != null) realHandModel.SetActive(true);
            Debug.Log("Mano liberada por el jugador (Grip o Drop).");
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
            // 1. Guardamos una referencia LOCAL al objeto que queremos destruir
            // Así, aunque 'currentEquippedObject' se vuelva null en el evento, 'tempObj' sigue vivo
            GameObject tempObj = currentEquippedObject;

            Debug.Log("Forzando desequipado de: " + tempObj.name);

            // 2. Si el interactor lo tiene seleccionado, lo soltamos legalmente
            if (handInteractor.hasSelection)
            {
                // Buscamos cuál de los objetos seleccionados es el nuestro
                var selected = handInteractor.interactablesSelected[0];

                // Al ejecutar esta línea, se disparará OnObjectReleased y currentEquippedObject será NULL
                handInteractor.interactionManager.SelectExit(handInteractor, selected);
            }

            // 3. Ahora destruimos la referencia local que guardamos al principio
            // Esto garantiza que el objeto desaparece del mundo
            Destroy(tempObj);

            // 4. Aseguramos que la global esté limpia
            currentEquippedObject = null;

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

        GameObject obj = handInteractor.interactablesSelected[0].transform.gameObject;

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