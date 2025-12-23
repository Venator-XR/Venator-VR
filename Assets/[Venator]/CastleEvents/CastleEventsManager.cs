using System.Collections;
using UnityEngine;

public class CastleEventsManager : MonoBehaviour
{
    // References in component
    DebrisSequence debrisSequence;
    WardrobeSequence wardrobeSequence;

    //-----------------------
    private bool woodLeverActioned = false;


    void Start()
    {
        debrisSequence = GetComponent<DebrisSequence>();
        wardrobeSequence = GetComponent<WardrobeSequence>();
    }

    // public void methods
    public void PlayDebrisSequence(float value)
    {

        if (value >= 0.6 && !woodLeverActioned)
        {
            Debug.Log("Wood Lever actioned, starting coroutine");
            woodLeverActioned = true;

            StartCoroutine(debrisSequence.DebrisCoroutine());
        }
    }

    public void PlayWardrobeSequence()
    {
        Debug.Log("Wardrobe actioned, starting coroutine");

        StartCoroutine(wardrobeSequence.WardrobeCoroutine());
    }



    //----------------------
    // COROUTINES
}
