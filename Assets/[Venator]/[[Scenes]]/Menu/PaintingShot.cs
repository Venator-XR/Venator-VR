using UnityEngine;

public class PaintingShot : MonoBehaviour
{
    public MainMenuActions mainMenuActions;

    public void Shot()
    {
        mainMenuActions.PlayGame();
    }
}
