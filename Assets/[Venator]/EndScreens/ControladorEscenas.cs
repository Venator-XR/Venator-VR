using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenActions : MonoBehaviour
{
    [Header("Scenes names")]
    public string finalScene = "Final";
    public string mainScene = "Main";
    public string menuScene = "Menu";

    [Header("Player Ref")]
    public GameObject player;

    public SceneTransition sceneTransition;

    IEnumerator Start() 
    {
        // Esperamos un frame para asegurar que los scripts de Awake han corrido
        yield return null; 

        // Opcional: Esperar un pelín más (0.1s) es mano de santo para evitar conflictos con el tracking
        yield return new WaitForSeconds(0.05f); 

        if (player != null)
        {
            // Ahora sí, forzamos el TP
            Debug.Log("Auto-Teleporting Player to Start Position");
            player.GetComponent<PlayerMobilityManager>().ForceTeleport(gameObject.transform);
        }
    }

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