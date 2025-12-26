using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainSceneName = "Main";
    [SerializeField] private string videoSceneName = "Video360";

    [Header("Settings")]
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] Transform bigCog;
    [SerializeField] Transform smallCog1;
    [SerializeField] Transform smallCog2;
    [SerializeField] float rotationSpeed = 100f;

    private bool settings = false;

    void Start()
    {
        Application.targetFrameRate = 90;
        // activate vSync
        QualitySettings.vSyncCount = 1;
    }

    // Se ejecuta cada frame
    private void Update()
    {
        if (settings)
        {
            // Calculate how much to rotate cogs
            float step = rotationSpeed * Time.deltaTime;

            // Rotate big cog
            bigCog.Rotate(0, 0, step);

            // Rotate small cogs on reverse and x2 fast
            smallCog1.Rotate(0, 0, -step * 2);
            smallCog2.Rotate(0, 0, -step * 2);
        }
    }

    // Change to Game Scene
    public void PlayGame()
    {
        Debug.Log("[MENU] PlayGame -> " + mainSceneName);
        SceneManager.LoadScene(mainSceneName);
    }

    // 360 Video Scene
    public void VideoPlay()
    {
        Debug.Log("[MENU] VideoPlay -> " + videoSceneName);
        SceneManager.LoadScene(videoSceneName);
    }

    // Settings
    public void ToggleSettings()
    {
        settingsCanvas.SetActive(!settings);
        settings = !settings;
    }

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