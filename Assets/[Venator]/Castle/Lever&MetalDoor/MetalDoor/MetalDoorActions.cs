using UnityEngine;

public class MetalDoorActions : MonoBehaviour
{
    private Animator animator;
    public Collider trigger;
    public GameObject otherDoor;
    public GameObject candelabra;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetTrigger("open");
        trigger.enabled = true;
        otherDoor.GetComponent<Animator>().SetTrigger("close1");
        candelabra.SetActive(false);
    }

    private void Close()
    {
        Debug.LogWarning("Metal door closed");
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
