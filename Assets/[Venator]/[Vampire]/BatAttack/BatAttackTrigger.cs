using UnityEngine;

public class BatAttackTrigger : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject batAttackPrefab;
    [SerializeField] private Transform batStartTransform;
    [SerializeField] private Transform objectiveTransform;

    [Header("SFXs")]
    [SerializeField] private AudioClip attackSFX;

    private AudioSource _audioSource;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _audioSource.PlayOneShot(attackSFX);
            Quaternion rotation = Quaternion.LookRotation(objectiveTransform.position - batStartTransform.position);
            Instantiate(batAttackPrefab, batStartTransform.position, rotation);
            Destroy(gameObject);
        }
    }
}
