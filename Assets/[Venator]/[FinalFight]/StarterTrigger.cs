using UnityEngine;

public class StarterTrigger : MonoBehaviour
{
    [SerializeField] FinalFightManager finalFightManager;
    [SerializeField] GameObject[] pastRooms;
    [SerializeField] GameObject darkness;
    
    void OnTriggerEnter(Collider other)
    {
        foreach (GameObject room in pastRooms) room.SetActive(false);
        if(darkness != null) darkness.SetActive(true);
        if(other.gameObject.CompareTag("Player")) finalFightManager.StartFight();
    }
}
