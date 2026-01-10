using UnityEngine;

public class BatAttackTrigger : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject batAttackPrefab;
    [SerializeField] private Transform batStartTransform;
    [SerializeField] private Transform objectiveTransform;

    [Header("SFXs")]
    [SerializeField] private AudioClip attackSFX;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<AudioSource>().PlayOneShot(attackSFX);
            Quaternion rotation = Quaternion.LookRotation(objectiveTransform.position - batStartTransform.position);
            Instantiate(batAttackPrefab, batStartTransform.position, rotation);
            Destroy(gameObject);
        }
    }
}
