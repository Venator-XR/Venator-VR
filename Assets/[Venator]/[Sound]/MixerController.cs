using UnityEngine;
using UnityEngine.Audio; // Necesario para usar AudioMixer

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;

    public void SetGroupVolume(string group, float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20;
        
        myMixer.SetFloat(group, dB);
    }
}