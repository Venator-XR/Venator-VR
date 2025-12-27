using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LockedInteractable : XRGrabInteractable
{
    // override drop method to stop it from happening
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // empty
    }

    // public method to force object drop
    public void ForceDrop(IXRSelectInteractor interactor)
    {
        base.OnSelectExiting(new SelectExitEventArgs 
        { 
            interactorObject = interactor,
            interactableObject = this,
            manager = interactionManager
        });
    }
}