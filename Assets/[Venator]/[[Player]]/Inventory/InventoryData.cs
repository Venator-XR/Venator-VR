using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string ID;
    public Sprite icon;
    public GameObject modelPrefab;
    public bool isDroppable = true;
}