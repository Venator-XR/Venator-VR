using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VRFootstepController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float stepDistance = 0.5f;
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float pitchRange = 0.1f;

    [Header("SFXs")]
    [SerializeField] private AudioClip[] defaultClips;
    [SerializeField] private AudioClip[] runningClips;

    public bool isRunning = false;

    private CharacterController _cc;
    public AudioSource audioSource;
    private float _distanceTravelled;

    private Vector3 _lastPosition;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        _lastPosition = transform.position;
    }

    void Update()
    {
        CheckFootsteps();
    }

    private void CheckFootsteps()
    {
        Vector3 currentPos = transform.position;
        Vector3 displacement = currentPos - _lastPosition;
        displacement.y = 0;

        float distanceThisFrame = displacement.magnitude;

        float speed = distanceThisFrame / Time.deltaTime;

        if (speed > minSpeed)
        {
            _distanceTravelled += distanceThisFrame;

            if (_distanceTravelled >= stepDistance)
            {
                PlayFootstep();
                _distanceTravelled = 0f;
            }
        }
        else
        {
            _distanceTravelled = stepDistance * 0.9f;
        }

        _lastPosition = currentPos;
    }

    private void PlayFootstep()
    {
        AudioClip[] clips;
        if (!isRunning) clips = defaultClips;
        else clips = runningClips;
        if (clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.volume = Random.Range(0.8f, 1.0f);

        audioSource.PlayOneShot(clip);
    }
}