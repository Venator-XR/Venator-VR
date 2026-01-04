using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class MetalDoorActions : MonoBehaviour
{
    private Animator animator;
    public Collider trigger;
    public GameObject otherDoor;
    public GameObject candelabra;
    public XRKnobDoor finalDoorScript;
    public GameObject mainHall;
    public GameObject[] pastRooms;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        mainHall.SetActive(true);
        animator.SetTrigger("open");
        trigger.enabled = true;
        otherDoor.GetComponent<Animator>().SetTrigger("close1");
        candelabra.SetActive(false);
    }

    private void Close()
    {
        Debug.LogWarning("Metal door closed");
        finalDoorScript.enabled = true;
        foreach(GameObject room in pastRooms) room.SetActive(false);
        animator.SetTrigger("close");
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Close();
            trigger.enabled = false;
        }
    }
}
