using UnityEngine;

public class StarterTrigger : MonoBehaviour
{
    [SerializeField] FinalFightManager finalFightManager;
    [SerializeField] GameObject[] pastRooms;
    [SerializeField] GameObject darkness;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (GameObject room in pastRooms) room.SetActive(false);
            if (darkness != null) darkness.SetActive(true);
            finalFightManager.StartFight();
        }
    }
}

