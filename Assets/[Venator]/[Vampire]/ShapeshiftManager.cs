using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ShapeshiftManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject vampireForm;
    [SerializeField] GameObject batForm;
    [SerializeField] ParticleSystem shapeshiftPS;
    

    [Header("Shapeshift Variables")]
    [SerializeField] int shapeshiftDelayMs = 600; // its in milliseconds
    public ShapeState currentForm;

    [Header("SFX")]
    [SerializeField] private AudioClip sfx;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (vampireForm == null) Debug.LogError("vampireForm not assigned in the inspector");
        if (batForm == null) Debug.LogError("batForm not assigned in the inspector");
        if (shapeshiftPS == null) Debug.LogError("shapeshiftPS not assigned in the inspector");
    }

    public async void Shapeshift()
    {
        shapeshiftPS.Play();
        audioSource.PlayOneShot(sfx);        

        await Task.Delay(shapeshiftDelayMs);

        if (currentForm == ShapeState.Vampire)
        {
            vampireForm.SetActive(false);
            batForm.SetActive(true);
            currentForm = ShapeState.Bat;
        }
        else
        {
            batForm.SetActive(false);
            vampireForm.SetActive(true);
            currentForm = ShapeState.Vampire;
        }
    }
}
