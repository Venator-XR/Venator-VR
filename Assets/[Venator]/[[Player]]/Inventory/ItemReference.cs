using UnityEngine;

public class ItemReference : MonoBehaviour
{
    [Tooltip("Drag here the ScriptableObject (InventoryItemData) corresponding to this object")]
    public InventoryItemData data;
}