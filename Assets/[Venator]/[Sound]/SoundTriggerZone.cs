using UnityEngine;

public class SoundTriggerZone : MonoBehaviour
{
    [Header("Configuración del Sonido")]
    [Tooltip("Arrastra aquí el clip de audio que quieres que suene.")]
    public AudioClip soundClip;

    [Range(0f, 1f)]
    public float volume = 1.0f;

    [Tooltip("Si es true, el sonido solo se reproducirá la primera vez.")]
    public bool playOnce = false;

    [Header("Referencias")]
    [Tooltip("Este es el objeto hijo 'Altavoz'. El script intentará encontrarlo automáticamente.")]
    public AudioSource speakerSource;

    private bool hasPlayed = false;

    private void Awake()
    {
        // 1. Si el líder olvidó asignar el 'speaker', lo buscamos en los hijos
        if (speakerSource == null)
        {
            speakerSource = GetComponentInChildren<AudioSource>();
        }

        // 2. Configuración de seguridad para asegurar que sea 3D
        if (speakerSource != null)
        {
            speakerSource.playOnAwake = false;
            speakerSource.spatialBlend = 1.0f; // Fuerza el sonido a ser 3D (0 es 2D, 1 es 3D)
        }
        else
        {
            Debug.LogError($"¡Ojo! El trigger {name} no tiene un AudioSource hijo asignado.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // En VR, asegúrate de que el objeto que tiene el AudioListener (la cámara)
        // o el cuerpo del jugador tenga el tag "Player".
        if (other.CompareTag("Player"))
        {
            if (playOnce && hasPlayed) return;

            if (speakerSource != null && soundClip != null)
            {
                // Asignamos el clip y reproducimos
                speakerSource.clip = soundClip;
                speakerSource.volume = volume;
                speakerSource.Play();
                
                hasPlayed = true;
            }
        }
    }
}