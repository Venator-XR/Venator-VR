using System.Collections;
using UnityEngine;

public class CastleEventsManager : MonoBehaviour
{
    // References in component
    DebrisSequence debrisSequence;

    //-----------------------
    private bool woodLeverActioned = false;


    void Start()
    {
        debrisSequence = this.GetComponent<DebrisSequence>();
    }

    // public void methods
    public void PlayDebrisSequence(float value)
    {

        if (value >= 0.6 && !woodLeverActioned)
        {
            Debug.Log("Wood Lever actioned, starting coroutine");
            woodLeverActioned = true;

            debrisSequence.StartDebrisCoroutine();
        }
    }

    public void PlayWardrobeSequence()
    {
        Debug.Log("Wardrobe actioned, starting coroutine");

        // StartWardrobeCoroutine
    }



    //----------------------
    // COROUTINES
}
