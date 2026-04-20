using System.Collections;
using UnityEngine;

public class BatAttackTrigger : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject batAttackPrefab;
    [SerializeField] private Transform batStartTransform;
    [SerializeField] private Transform objectiveTransform;
    [SerializeField] private GameObject shakeTutorial;

    [Header("SFXs")]
    [SerializeField] private AudioClip attackSFX;

    private AudioSource _audioSource;
    private Collider _collider;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _audioSource.PlayOneShot(attackSFX);
            Quaternion rotation = Quaternion.LookRotation(objectiveTransform.position - batStartTransform.position);
            Instantiate(batAttackPrefab, batStartTransform.position, rotation);
            _collider.enabled = false;
            StartCoroutine(Shake());
            Destroy(gameObject, 10f);
        }
    }

    private IEnumerator Shake()
    {
        yield return new WaitForSeconds(4f);

        if (shakeTutorial != null)
        {
            shakeTutorial.SetActive(true);
            Debug.Log("ShakeTutorial");
        }

    }
}
