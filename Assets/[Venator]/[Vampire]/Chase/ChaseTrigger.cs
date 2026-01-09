using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    public VampireChaseManager vampireChaseManager;
    public bool approachStarted = false;

    void OnTriggerEnter(Collider other)
    {
        if (approachStarted) return;
        if(other.CompareTag("Player"))
        {
            approachStarted = true;
            vampireChaseManager.StartApproach();
        }
    }
}
