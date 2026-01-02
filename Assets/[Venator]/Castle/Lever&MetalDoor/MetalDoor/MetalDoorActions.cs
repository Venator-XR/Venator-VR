using UnityEngine;

public class MetalDoorActions : MonoBehaviour
{
    private Animator animator;
    public Collider trigger;
    public GameObject otherDoor;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetTrigger("open");
        trigger.enabled = true;
        otherDoor.transform.position = new Vector3(otherDoor.transform.position.x, otherDoor.transform.position.y, 0);
    }

    private void Close()
    {
        animator.SetTrigger("close");
    }

    void OnTriggerExit(Collider other)
    {
        Close();
    }
}
