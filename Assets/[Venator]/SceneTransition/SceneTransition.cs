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

        // Load scene on background
        Debug.Log("loading scene");
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        // Change scene after its loaded
        while (!operation.isDone)
        {
            Debug.Log("scene loaded");
            yield return null;
        }
    }

    public IEnumerator FinalRoutine(bool victory)
    {
        yield return StartCoroutine(SlowFadeIn());

        // Load scene on background
        Debug.Log("loading scene");
        AsyncOperation operation;
        if (victory)
        {
            operation = SceneManager.LoadSceneAsync("Victory");
        }
        else
        {
            operation = SceneManager.LoadSceneAsync("Defeat");
        }
        // Change scene after its loaded
        while (!operation.isDone)
        {
            Debug.Log("scene loaded");
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
