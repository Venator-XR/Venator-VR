using UnityEngine;

public class ChangePainting : MonoBehaviour
{
    public Material normalMaterial;
    public Material scaryMaterial;
    public AudioSource scareSource;
    
    private MeshRenderer m_Renderer;
    private bool hasChanged = false;

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Renderer.material = normalMaterial;
    }

    public void Change()
    {
        m_Renderer.material = scaryMaterial;
        scareSource.Play();
        Debug.Log("Painting changed!");
    }
}