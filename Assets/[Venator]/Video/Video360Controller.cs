using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Video360Controller : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;
    
    [Header("Scene Settings")]
    [SerializeField] private string menuSceneName = "Menu";

    private void Awake()
    {
        // Si no se asigna el VideoPlayer, intentar encontrarlo automáticamente
        if (videoPlayer == null)
        {
            videoPlayer = FindAnyObjectByType<VideoPlayer>();
            if (videoPlayer == null)
            {
                Debug.LogError("[Video360Controller] No se encontró un VideoPlayer en la escena!");
            }
        }
    }

    private void Start()
    {
        // Suscribirse al evento de finalización del video
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnded;
        }
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnded;
        }
    }

    /// <summary>
    /// Se llama cuando el video termina
    /// </summary>
    private void OnVideoEnded(VideoPlayer source)
    {
        Debug.Log("[Video360] Video finalizado, volviendo al menú");
        ReturnToMenu();
    }

    /// <summary>
    /// Reproduce el video
    /// </summary>
    public void PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            Debug.Log("[Video360] Video reproducido");
        }
    }

    /// <summary>
    /// Pausa el video
    /// </summary>
    public void PauseVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
            Debug.Log("[Video360] Video pausado");
        }
    }

    /// <summary>
    /// Reinicia el video al segundo 0
    /// </summary>
    public void ResetVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.time = 0;
            videoPlayer.Pause();
            Debug.Log("[Video360] Video reiniciado al segundo 0");
        }
    }

    /// <summary>
    /// Alterna entre play y pausa
    /// </summary>
    public void TogglePlayPause()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                PauseVideo();
            }
            else
            {
                PlayVideo();
            }
        }
    }

    /// <summary>
    /// Vuelve a la escena del menú
    /// </summary>
    public void ReturnToMenu()
    {
        Debug.Log("[Video360] Volviendo al menú: " + menuSceneName);
        SceneManager.LoadScene(menuSceneName);
    }
}
