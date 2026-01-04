using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleEventsManager : MonoBehaviour
{
    // References in component
    DebrisSequence debrisSequence;
    WardrobeSequence wardrobeSequence;

    //-----------------------
    private bool woodLeverActioned = false;
    private bool wardrobeActioned = false;
    //-----------------------
    public string finalScene = "Final";
    public Animator transition;

    void Start()
    {
        debrisSequence = GetComponent<DebrisSequence>();
        wardrobeSequence = GetComponent<WardrobeSequence>();
    }

    public void PlayDebrisSequence(float value)
    {

        if (value >= 0.4 && !woodLeverActioned)
        {
            Debug.Log("Wood Lever actioned, starting coroutine");
            woodLeverActioned = true;

            StartCoroutine(debrisSequence.DebrisCoroutine());
        }
    }

    public void PlayWardrobeSequence(float value)
    {
        if (value >= 0.4 && !wardrobeActioned)
        {
            Debug.Log("Wardrobe actioned, starting coroutine");
            wardrobeActioned = true;

            StartCoroutine(wardrobeSequence.WardrobeCoroutine());
        }
    }

    public void ChangeScene(float value)
    {
        if (value > 0.7 || value < 0.3)
        {
            StartCoroutine(SceneChangeCoroutine());
        }
    }

    private IEnumerator SceneChangeCoroutine()
    {
        transition.Play("fadeIn");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(finalScene);
        yield break;
    }
}
