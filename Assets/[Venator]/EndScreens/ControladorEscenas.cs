using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenActions : MonoBehaviour
{
    [Header("Scenes names")]
    public string finalScene = "Final";
    public string mainScene = "Main";
    public string menuScene = "Menu";

    public SceneTransition sceneTransition;

    public void PlayAgain()
    {
        Time.timeScale = 1; // Importante: Quitar la pausa antes de recargar
        CheckpointState.SpawnAtCheckpoint = true;
        if (CheckpointState.FinalSceneReached)
            StartCoroutine(sceneTransition.ChangeSceneRoutine(finalScene));
        else
            StartCoroutine(sceneTransition.ChangeSceneRoutine(mainScene));
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1;
            StartCoroutine(sceneTransition.ChangeSceneRoutine(menuScene));
    }

    public void ExitGame()
    {
        Application.Quit(); // Esto cierra el .exe o la app (no funciona en el editor)
        Debug.Log("Exiting game...");
    }
}