using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject flashlightText;
    [SerializeField] GameObject inventoryText1;
    [SerializeField] GameObject inventoryText2;
    [SerializeField] GameObject inventoryText3;
    [SerializeField] GameObject inventoryText4;

    [Header("References")]
    public PlayerMobilityManager playerMobilityManager;
    public InventoryController inventoryController;
    public FlashlightController flashlightController;
    public HandEquipmentManager handManager;
    public InventoryItemData pistolData;


    void Awake()
    {
        // Deactivate canvas with initial text active
        flashlightText.SetActive(true);
        inventoryText1.SetActive(false);
        inventoryText2.SetActive(false);
        inventoryText3.SetActive(false);
        inventoryText4.SetActive(false);
        canvas.SetActive(false);
        inventoryController.enabled = false;
    }

    //-----------------------------------------------------
    // Subscribe / Unsuscribe
    private void OnEnable()
    {
        if (inventoryController != null)
        {
            inventoryController.OnInventoryOpened += HandleInventoryOpened;
            inventoryController.OnInventoryClosed += HandleInventoryClosed;
        }
        if (flashlightController != null)
        {
            flashlightController.OnFlashlightToggle += FlashlightTutorial;
        }

        canvas.SetActive(true);
    }

    private void OnDisable()
    {
        // NOS DESUSCRIBIMOS (Importante para evitar errores)
        if (inventoryController != null)
        {
            inventoryController.OnInventoryOpened -= HandleInventoryOpened;
            inventoryController.OnInventoryClosed -= HandleInventoryClosed;
        }
    }
    //-----------------------------------------------------

    private void FlashlightTutorial()
    {
        flashlightText.SetActive(false);

        // enable inventory to continue tutorial
        inventoryText1.SetActive(true);
        inventoryController.enabled = true;
    }

    // Inventory Opened
    private void HandleInventoryOpened()
    {
        // activate second text
        inventoryText1.SetActive(false);
        inventoryText2.SetActive(true);

        // just to be sure
        inventoryText3.SetActive(false);
    }

    // Inventory Closed
    private void HandleInventoryClosed()
    {
        // Get current item
        InventoryItemData currentItem = handManager.GetHeldItemData();

        if (currentItem == pistolData)
        {
            // Next step (tutorial complete)
            inventoryText2.SetActive(false);
            inventoryText3.SetActive(true);
            
            StartCoroutine(DestroyTutorial());
        }
        else
        {
            // reset texts if player did NOT held pistol
            inventoryText1.SetActive(true);
            inventoryText2.SetActive(false);
        }
    }

    private IEnumerator DestroyTutorial()
    {
        playerMobilityManager.SetPlayerMobility(true, true);
        yield return new WaitForSeconds(10f);
        inventoryText3.SetActive(false);
        inventoryText4.SetActive(true);
        yield return new WaitForSeconds(10f);
        inventoryText4.SetActive(false);
        
        Destroy(canvas);
        Destroy(gameObject); 
    }
}
