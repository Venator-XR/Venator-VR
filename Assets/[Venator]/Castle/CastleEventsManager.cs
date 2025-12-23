using System.Collections;
using UnityEngine;

public class CastleEventsManager : MonoBehaviour
{

    // public void methods
    public void PlayDebrisSequence()
    {
        Debug.Log("Wood Lever actioned, starting coroutine");

        // StartDebrisCoroutine();
    }

    public void PlayWardrobeSequence()
    {
        Debug.Log("Wardrobe actioned, starting coroutine");

        // StartWardrobeCoroutine
    }


    //----------------------
    // private 
    private IEnumerator StartDebrisCoroutine()
    {
        // disable movement, camera turning and collider

        // fade to black

        // move player smoothly through with surrounding sfx of debris falling

        // also play steps and wood creaking

        // finally play sfx of rocks and wood falling

        // activate debris game object

        // player should be in the correct location and looking torwards wardrobe

        // wait x seconds

        // fade from black

        // enable movement, camera turning and collider

        // play lightning animation + sfx

        yield break;
    }

    private IEnumerator StartWardrobeCoroutine()
    {
        // disable movement, camera turning and collider

        // fade to black

        // tp player inside wardrobe looking through the hole

        // play sfx: wardrobe opening | steps | wardrobe closing

        // fade from black

        // start vampire animation:
        // transform into bat
        // fly through hole in debris
        // transform back inside the room where player clearly sees
        // leave through door closing it behind
        
        // wait 2 seconds

        // fade to black

        // tp player outside wardrobe looking at door

        // play sfx: wardrobe opening | steps | wardrobe closing

        // fade from black

        // StartInventoryTutorial();

        yield break;
    }

    private void StartInventoryTutorial()
    {
        // enable canvas with first text

        // detect opening of inventory

        // enable second text

        // detect pistol equipped 

        // disable canvas
    }
}
