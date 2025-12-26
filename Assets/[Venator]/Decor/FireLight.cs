using UnityEngine;

public class FireLight : MonoBehaviour
{
    public Light[] lights;
    private float baseIntensity = .1f;
    public float flickerSpeed = 5f;
    public float flickerAmount = 0.3f;

    void Start()
    {
        if(lights.Length == 0)
        {
            Debug.LogError("no lights assigned");
        }
        
        if(lights.Length > 0)
        {
            baseIntensity = lights[0].intensity;
        }
    }

    void Update()
    {
        float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerAmount;
        
        foreach(Light l in lights)
        {
            l.intensity = baseIntensity + flicker;
        }
    }
}