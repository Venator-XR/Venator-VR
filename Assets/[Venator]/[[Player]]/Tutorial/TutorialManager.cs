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
    public GameObject inventoryGameObj;
    public FlashlightController flashlightController;
    public HandEquipmentManager handManager;
    public InventoryItemData pistolData;

    //--------------------------
    private int step = 1;


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
        if (step > 1) return;
        flashlightText.SetActive(false);

        // enable inventory to continue tutorial
        step = 2;
        inventoryText1.SetActive(true);
        inventoryController.enabled = true;
        inventoryGameObj.SetActive(true);
    }

    // Inventory Opened
    private void HandleInventoryOpened()
    {
        if (step < 3)
        {
            // activate second text
            inventoryText1.SetActive(false);
            inventoryText2.SetActive(true);
        }
    }

    // Inventory Closed
    private void HandleInventoryClosed()
    {
        // Get current item
        InventoryItemData currentItem = handManager.GetHeldItemData();

        if (currentItem == pistolData)
        {
            step = 3;
            // Next step (tutorial complete)
            inventoryText2.SetActive(false);
            inventoryText3.SetActive(true);

            StartCoroutine(DestroyTutorial());
        }
        else
        {
            if (step < 3)
            {
                // reset texts if player did NOT held pistol
                inventoryText1.SetActive(true);
                inventoryText2.SetActive(false);
            }
        }
    }

    private IEnumerator DestroyTutorial()
    {
        playerMobilityManager.SetPlayerMobility(true, true);
        yield return new WaitForSeconds(6f);
        inventoryText3.SetActive(false);
        inventoryText4.SetActive(true);
        yield return new WaitForSeconds(6f);
        inventoryText4.SetActive(false);

        Destroy(canvas);
        Destroy(gameObject);
    }
}
