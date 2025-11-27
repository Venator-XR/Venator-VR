using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainSceneName = "Main"; 
    [SerializeField] private string videoSceneName = "Video360";

    // 1. Cambiar a la escena principal
    public void PlayGame()
    {
        Debug.Log("[MENU] PlayGame -> " + mainSceneName);
        SceneManager.LoadScene(mainSceneName);
    }

    // Escena video 360 (Trailer)
    public void VideoPlay()
    {
        Debug.Log("[MENU] VideoPlay -> " + videoSceneName);
        SceneManager.LoadScene(videoSceneName);
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
