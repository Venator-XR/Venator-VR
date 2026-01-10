using UnityEngine;

public class BatAttackTrigger : MonoBehaviour
{
    [SerializeField] private GameObject batAttackPrefab;
    [SerializeField] private Transform batStartTransform;
    [SerializeField] private Transform objectiveTransform;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Quaternion rotation = Quaternion.LookRotation(objectiveTransform.position - batStartTransform.position);
            Instantiate(batAttackPrefab, batStartTransform.position, rotation);
            Destroy(gameObject);
        }
    }
}
