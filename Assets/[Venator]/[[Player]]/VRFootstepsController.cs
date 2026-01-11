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

    public CharacterController _cc;
    private AudioSource _audioSource;
    private float _distanceTravelled;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckFootsteps();
    }

    private void CheckFootsteps()
    {
        // 1. Si no estamos en el suelo, no suenan pasos (evita sonido al caer)
        if (!_cc.isGrounded) return;

        // 2. Calculamos la velocidad horizontal (ignoramos movimiento vertical Y)
        Vector2 horizontalVelocity = new Vector2(_cc.velocity.x, _cc.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // 3. Si nos movemos
        if (speed > minSpeed)
        {
            // Acumulamos distancia basada en cuanto nos hemos movido este frame
            _distanceTravelled += speed * Time.deltaTime;

            // 4. Si hemos superado la distancia de un paso
            if (_distanceTravelled >= stepDistance)
            {
                PlayFootstep();
                _distanceTravelled = 0f; // Reseteamos contador
            }
        }
        else
        {
            // Opcional: Si nos paramos, reseteamos la distancia para que el
            // primer paso al arrancar suene casi inmediato o mantenga el ritmo.
            _distanceTravelled = stepDistance * 0.9f; 
        }
    }

    private void PlayFootstep()
    {
        if (defaultClips.Length == 0) return;

        // Elegir clip aleatorio
        AudioClip clip = defaultClips[Random.Range(0, defaultClips.Length)];
        
        // Variación de Pitch (CRUCIAL para que no suene a metralleta robótica)
        _audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        _audioSource.volume = Random.Range(0.8f, 1.0f); // Leve variación volumen
        
        _audioSource.PlayOneShot(clip);
    }
}