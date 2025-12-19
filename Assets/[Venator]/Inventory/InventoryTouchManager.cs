using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTouchManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Transform spawnPoint;

    [Header("Referencias a los Slots (Scripts)")]
    // Arrastra aquí los objetos que tienen el script InventorySlot
    [SerializeField] private InventorySlot slotScriptLeft;
    [SerializeField] private InventorySlot slotScriptRight;
    [SerializeField] private InventorySlot slotScriptDown;

    [Header("Objetos Equipables")]
    [SerializeField] private GameObject modelPistol;
    [SerializeField] private GameObject modelLeftItem;
    [SerializeField] private GameObject modelHand;

    [Header("Input")]
    [SerializeField] private InputActionProperty toggleButton;

    private bool isInventoryOpen = false;
    private int currentSelectedID = -1;

    void Start()
    {
        // 1. INICIALIZACIÓN MANUAL (Soluciona el error NullReference)
        // El manager se presenta a sus empleados
        if (slotScriptLeft != null) slotScriptLeft.Setup(this);
        if (slotScriptRight != null) slotScriptRight.Setup(this);
        if (slotScriptDown != null) slotScriptDown.Setup(this);

        // IMPORTANTE: Al hacerlo manual, hay que encender la escucha
        if (toggleButton != null)
            toggleButton.action.Enable();

        uiCanvas.SetActive(false);
        EquipHand();
    }

    void Update()
    {
        // PRUEBA DE INPUT: Si sale este mensaje, el botón funciona
        if (toggleButton.action.WasPressedThisFrame())
        {
            Debug.Log("BOTÓN 'A' DETECTADO: Abriendo inventario...");
            OpenInventory();
        }

        if (toggleButton.action.WasReleasedThisFrame())
        {
            CloseAndEquip();
        }
    }

    private void OpenInventory()
    {
        isInventoryOpen = true;

        // Colocamos el canvas en el spawn point
        uiCanvas.transform.position = spawnPoint.position;
        // Hacemos que mire hacia la cámara (opcional, pero ayuda a leerlo)
        uiCanvas.transform.LookAt(Camera.main.transform);
        // Invertimos rotación si sale al revés (o simplemente copia la rotación de la mano)
        // uiCanvas.transform.rotation = spawnPoint.rotation; 

        uiCanvas.SetActive(true);
    }

    private void CloseAndEquip()
    {
        if (!isInventoryOpen) return;

        // CHIVATO 1: ¿Qué ID tenemos guardado al soltar?
        Debug.Log($"[INVENTARIO] Soltando botón A. ID Seleccionado: {currentSelectedID}");

        isInventoryOpen = false;
        uiCanvas.SetActive(false);

        switch (currentSelectedID)
        {
            case 0:
                Debug.Log("[INVENTARIO] Intentando equipar IZQUIERDA");
                EquipLeft();
                break;
            case 1:
                Debug.Log("[INVENTARIO] Intentando equipar PISTOLA");
                EquipPistol();
                break;
            case 2:
                Debug.Log("[INVENTARIO] Intentando equipar MANO");
                EquipHand();
                break;
            default:
                Debug.Log("[INVENTARIO] Nada seleccionado (-1). Manteniendo estado anterior.");
                break;
        }

        // Reseteamos colores
        if (slotScriptLeft) slotScriptLeft.ResetColor();
        if (slotScriptRight) slotScriptRight.ResetColor();
        if (slotScriptDown) slotScriptDown.ResetColor();

        currentSelectedID = -1;
    }

    public void SetCurrentSelection(int id) => currentSelectedID = id;

    public void ClearSelection(int id)
    {
        if (currentSelectedID == id) currentSelectedID = -1;
    }

    // --- Equipamiento ---
    private void DisableAllModels()
    {
        if (modelPistol) modelPistol.SetActive(false);
        if (modelLeftItem) modelLeftItem.SetActive(false);
        if (modelHand) modelHand.SetActive(true);
    }

    private void EquipPistol()
    {
        DisableAllModels();
        if (modelHand) modelHand.SetActive(false);
        if (modelPistol) modelPistol.SetActive(true);
    }

    private void EquipLeft()
    {
        DisableAllModels();
        if (modelLeftItem) modelLeftItem.SetActive(true);
    }

    private void EquipHand()
    {
        DisableAllModels();
    }
}