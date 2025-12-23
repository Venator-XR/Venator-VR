using UnityEngine;

public class OutlineTrigger : MonoBehaviour
{
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();

        if (outline == null) Debug.LogError("Outline.cs not found");
        else outline.enabled = false;

        Collider col = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (outline != null && other.CompareTag("Player"))
        {
            outline.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (outline != null && other.CompareTag("Player"))
        {
            outline.enabled = false;
        }
    }
}