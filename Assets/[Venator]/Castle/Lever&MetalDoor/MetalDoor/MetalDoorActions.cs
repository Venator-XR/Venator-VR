using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class MetalDoorActions : MonoBehaviour
{
    [Header("Refs")]
    private Animator animator;
    public Collider trigger;
    public GameObject otherDoor;
    public GameObject candelabra;
    public XRKnobDoor finalDoorScript;
    public GameObject mainHall;
    public GameObject[] pastRooms;

    [Header("SFXs")]
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;
    private AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Open()
    {
        mainHall.SetActive(true);
        audioSource.PlayOneShot(openSFX);
        animator.SetTrigger("open");
        trigger.enabled = true;
        otherDoor.GetComponent<Animator>().SetTrigger("close1");
        candelabra.SetActive(false);
    }

    private void Close()
    {
        Debug.LogWarning("Metal door closed");
        audioSource.PlayOneShot(closeSFX);
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
