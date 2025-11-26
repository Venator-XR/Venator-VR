using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainSceneName = "Main"; 

    // 1. Cambiar a la escena principal
    public void PlayGame()
    {
        Debug.Log("[MENU] PlayGame -> " + mainSceneName);
        SceneManager.LoadScene(mainSceneName);
    }

    // 2. Cerrar el juego
    public void QuitGame()
    {
        Debug.Log("[MENU] QuitGame");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
