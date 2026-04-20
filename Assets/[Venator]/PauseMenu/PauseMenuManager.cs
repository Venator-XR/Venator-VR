using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Main References")]
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _pauseCanvas;
    [SerializeField] GameObject _settingsCanvas;

    [Header("Player")]
    [SerializeField] NearFarInteractor nearFarInteractor;
    [SerializeField] PlayerMobilityManager playerMobilityManager;

    private bool _paused = false;

    [Header("Input")]
    public InputActionProperty pauseButton;

    void Start()
    {
        if (pauseButton.action != null)
            pauseButton.action.Enable();
    }

    void Update()
    {
        if (pauseButton != null && pauseButton.action.WasPressedThisFrame())
        {
            if (!_paused) Pause();
            else Unpause();
        }
    }

    private void Pause()
    {
        _paused = true;
        Time.timeScale = 0.0001f;
        playerMobilityManager.SetPlayerMobility(false, false);
        OpenPauseMenu();
    }

    private void Unpause()
    {
        _paused = false;
        Time.timeScale = 1f;
        playerMobilityManager.SetPlayerMobility(true, true);
        // undo what is done on Pause()
        ClosePauseMenu();
    }

    public void ClosePauseMenu()
    {
        nearFarInteractor.enableFarCasting = false;
        _settingsCanvas.SetActive(false);
        _pauseCanvas.SetActive(true);
        _pauseMenu.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        nearFarInteractor.enableFarCasting = true;
        _settingsCanvas.SetActive(false);
        _pauseCanvas.SetActive(true);
        _pauseMenu.SetActive(true);
    }

    public void OpenSettings()
    {
        _settingsCanvas.SetActive(true);
        _pauseCanvas.SetActive(false);
    }

    public void CloseSettings()
    {
        _settingsCanvas.SetActive(false);
        _pauseCanvas.SetActive(true);
    }
}
