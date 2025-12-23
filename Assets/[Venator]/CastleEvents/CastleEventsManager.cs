using System.Collections;
using UnityEngine;

public class CastleEventsManager : MonoBehaviour
{
    // References in component
    DebrisSequence debrisSequence;
    WardrobeSequence wardrobeSequence;

    //-----------------------
    private bool woodLeverActioned = false;
    private bool wardrobeActioned = false;


    void Start()
    {
        debrisSequence = GetComponent<DebrisSequence>();
        wardrobeSequence = GetComponent<WardrobeSequence>();
    }

    public void PlayDebrisSequence(float value)
    {

        if (value >= 0.6 && !woodLeverActioned)
        {
            Debug.Log("Wood Lever actioned, starting coroutine");
            woodLeverActioned = true;

            StartCoroutine(debrisSequence.DebrisCoroutine());
        }
    }

    public void PlayWardrobeSequence(float value)
    {
        if (value >= 0.6 && !wardrobeActioned)
        {
            Debug.Log("Wardrobe actioned, starting coroutine");
            wardrobeActioned = true;

            StartCoroutine(wardrobeSequence.WardrobeCoroutine());
        }
    }
}
