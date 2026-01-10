using UnityEngine;

public class BatAttack : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody _rb;
    private bool _flashlightTurnedOff = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    private void HandleImpact(GameObject hitObject)
    {
        if (hitObject.CompareTag("Player"))
        {
            if (!_flashlightTurnedOff)
            {
                FlashlightController flashlight = hitObject.GetComponentInChildren<FlashlightController>();
                flashlight.Malfunction(false);
                Debug.Log("Flashlight off");

                _flashlightTurnedOff = true;
            }
        }

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();

        if (ps != null)
        {
            ps.transform.parent = null;

            ps.Stop();

            float destroyDelay = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(ps.gameObject, destroyDelay);
        }

        Destroy(gameObject);
    }
}
