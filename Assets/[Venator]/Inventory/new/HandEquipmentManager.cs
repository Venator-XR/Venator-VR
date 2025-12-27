using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandEquipmentManager : MonoBehaviour
{
    [Header("Dependencies")]
    public XRBaseInteractor handInteractor; // Arrastra tu Direct Interactor
    public Transform itemSpawnPoint; // Un hijo vacío en la palma de la mano

    // Mantenemos el dato de qué hay en la mano
    private GameObject currentModel;
    private bool isLockedItem = false;

    public void EquipItem(InventoryItemData itemData)
    {
        // 1. Limpiar mano actual
        UnequipCurrent();

        if (itemData == null) return;

        // 2. Instanciar nuevo
        currentModel = Instantiate(itemData.modelPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
        
        // 3. Configurar físicas para que NO colisione al nacer
        // (Opcional: ignorar colisiones con el player momentáneamente)

        // 4. FORZAR AGARRE (La clave)
        var interactable = currentModel.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable != null)
        {
            handInteractor.interactionManager.SelectEnter((IXRSelectInteractor)handInteractor, interactable);
        }

        // 5. Guardar estado
        isLockedItem = !itemData.isDroppable;
    }

    public void UnequipCurrent()
    {
        if (currentModel == null) return;

        // Si es el objeto bloqueado (Pistola), usamos nuestro truco para soltarlo
        if (isLockedItem)
        {
            var locked = currentModel.GetComponent<LockedInteractable>();
            if (locked) locked.ForceDrop((IXRSelectInteractor)handInteractor);
        }
        else
        {
            // Soltado normal
            handInteractor.interactionManager.SelectExit((IXRSelectInteractor)handInteractor, currentModel.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>());
        }

        // Si estamos desequipando para guardar, destruimos el objeto visual
        // (Si quisiéramos tirarlo al suelo, no haríamos Destroy)
        Destroy(currentModel);
        currentModel = null;
    }
    
    // Método para soltar físicamente al suelo (Basura o Swap)
    public void DropToWorld()
    {
        if (currentModel == null || isLockedItem) return; // La pistola no se tira

        // Simplemente forzamos soltar, NO destruimos. Unity reactivará físicas.
         handInteractor.interactionManager.SelectExit((IXRSelectInteractor)handInteractor, currentModel.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>());
        
        currentModel.transform.SetParent(null); // Desvincular
        currentModel = null;
    }

    public bool IsHoldingItem() => handInteractor.hasSelection;
    
    // Devuelve el DATA del objeto que tenemos en la mano (si tiene)
    public InventoryItemData GetHeldItemData()
    {
        if (!IsHoldingItem()) return null;
        // Asumimos que los objetos del mundo tienen un componente simple "ItemReference"
        // Tienes que crear un script pequeñito "ItemReference" que solo tenga "public InventoryItemData data;"
        var obj = handInteractor.interactablesSelected[0].transform.gameObject;
        var refScript = obj.GetComponent<ItemReference>();
        return refScript ? refScript.data : null;
    }
}