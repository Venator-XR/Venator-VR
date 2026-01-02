using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    public VampireChaseManager vampireChaseManager;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            vampireChaseManager.StartApproach();
        }
    }
}
