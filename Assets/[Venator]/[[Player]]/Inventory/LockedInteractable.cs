using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NonDroppableGrab : XRGrabInteractable
{
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // block normal drop
        return;
    }

    // manual drop from code only
    public void ForceDrop(IXRSelectInteractor interactor)
    {
        base.OnSelectExiting(new SelectExitEventArgs
        {
            interactorObject = interactor,
            interactableObject = this,
            manager = interactionManager,
            isCanceled = true
        });
    }
}
