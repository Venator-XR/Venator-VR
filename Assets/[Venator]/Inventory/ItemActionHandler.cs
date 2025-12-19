using UnityEngine;
using UnityEngine.Events;

public class ItemActionHandler : MonoBehaviour
{
    // Arrastra aquí la función Toggle() de tu FlashlightController
    public UnityEvent onTriggerPress;

    public void OnAction()
    {
        onTriggerPress.Invoke();
    }
}