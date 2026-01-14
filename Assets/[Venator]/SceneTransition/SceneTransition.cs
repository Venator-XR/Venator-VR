using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator ChangeSceneRoutine(string scene)
    {
        // Start Fade Out
        yield return StartCoroutine(FadeIn());

        // Load scene async, but don't activate yet
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            // Scene is ready but not activated
            if (operation.progress >= 0.9f)
            {
                Debug.Log("Scene loaded, activating after fade");
                operation.allowSceneActivation = true; // Now actually switch
            }

            yield return null;
        }
    }


    public IEnumerator FinalRoutine(bool victory)
    {
        yield return StartCoroutine(SlowFadeIn());

        // Load scene async, but don't activate yet
        AsyncOperation operation;
        if (victory)
        {
            operation = SceneManager.LoadSceneAsync("Victory");
        }
        else
        {
            operation = SceneManager.LoadSceneAsync("Defeat");
        }
        operation.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            // Scene is ready but not activated
            if (operation.progress >= 0.9f)
            {
                Debug.Log("Scene loaded, activating after fade");
                operation.allowSceneActivation = true; // Now actually switch
            }

            yield return null;
        }
    }


    public IEnumerator FadeOut()
    {
        Debug.Log("FadeOut()");
        _animator.Play("fadeOut");

        yield return new WaitForSeconds(.5f);
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("FadeIn()");
        _animator.Play("fadeIn");

        yield return new WaitForSeconds(.5f);
    }

    public IEnumerator SlowFadeIn()
    {
        Debug.Log("SlowFadeIn()");
        _animator.SetTrigger("slowFadeIn");

        yield return new WaitForSeconds(3f);
    }
}
