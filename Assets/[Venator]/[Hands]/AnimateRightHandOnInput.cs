using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AnimateRightHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator HandAnimator;
    public string triggerName = "Trigger";
    public string gripName = "Grip";

    void Update()
    {
        float  triggerValue = pinchAnimationAction.action.ReadValue<float>();
        HandAnimator.SetFloat(triggerName, triggerValue);
        float  gripValue = gripAnimationAction.action.ReadValue<float>();
        HandAnimator.SetFloat(gripName, gripValue);
    }
}
