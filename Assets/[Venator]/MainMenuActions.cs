using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainSceneName = "Main";
    [SerializeField] private string videoSceneName = "Video360";

    [Header("Transition")]
    public Animator transition;

    [Header("Settings")]
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] Transform bigCog;
    [SerializeField] Transform smallCog1;
    [SerializeField] Transform smallCog2;
    [SerializeField] float rotationSpeed = 100f;

    public GameObject painting;
    private bool paintingActive = false;

    private bool settings = false;

    void Start()
    {
        Application.targetFrameRate = 90;
        // activate vSync
        QualitySettings.vSyncCount = 1;
    }

    private void Update()
    {
        // if settings animate cogs manually 
        if (settings)
        {
            // Calculate how much to rotate cogs
            float step = rotationSpeed * Time.deltaTime;

            // Rotate big cog
            bigCog.Rotate(0, 0, step);

            // Rotate small cogs on reverse and x2 so it aligns
            smallCog1.Rotate(0, 0, -step * 2);
            smallCog2.Rotate(0, 0, -step * 2);
        }
    }

    // Change to Game Scene
    public void PlayGame()
    {
        Debug.Log("[MENU] PlayGame -> " + mainSceneName);
        StartCoroutine(SceneChangeCoroutine(mainSceneName));
    }

    private IEnumerator SceneChangeCoroutine(string scene)
    {
        transition.Play("fadeIn");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(scene);
        yield break;
    }

    public void TogglePainting()
    {
        paintingActive = !paintingActive;
        painting.SetActive(paintingActive);
    }

    // 360 Video Scene
    public void VideoPlay()
    {
        Debug.Log("[MENU] VideoPlay -> " + videoSceneName);
        StartCoroutine(SceneChangeCoroutine(videoSceneName));
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