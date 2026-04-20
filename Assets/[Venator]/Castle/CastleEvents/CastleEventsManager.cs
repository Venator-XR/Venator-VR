using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleEventsManager : MonoBehaviour
{
    public GameObject player;
    // References in component
    DebrisSequence debrisSequence;
    WardrobeSequence wardrobeSequence;

    //-----------------------
    private bool woodLeverActioned = false;
    private bool wardrobeActioned = false;
    //-----------------------
    public string finalScene = "Final";
    bool sceneChanged = false;
    public SceneTransition sceneTransition;

    // Cambia 'void' por 'IEnumerator' para que Unity lo trate como corrutina automática
    IEnumerator Start() 
    {
        // Esperamos un frame para asegurar que los scripts de Awake han corrido
        yield return null; 

        // Opcional: Esperar un pelín más (0.1s) es mano de santo para evitar conflictos con el tracking
        yield return new WaitForSeconds(0.05f); 

        if (player != null)
        {
            // Ahora sí, forzamos el TP
            Debug.Log("Auto-Teleporting Player to Start Position");
            player.GetComponent<PlayerMobilityManager>().ForceTeleport(gameObject.transform);
        }

        // Inicializamos el resto
        debrisSequence = GetComponent<DebrisSequence>();
        wardrobeSequence = GetComponent<WardrobeSequence>();
    }

    public void PlayDebrisSequence(float value)
    {

        if (value >= 0.4 && !woodLeverActioned)
        {
            Debug.Log("Wood Lever actioned, starting coroutine");
            woodLeverActioned = true;

            StartCoroutine(debrisSequence.DebrisCoroutine());
        }
    }

    public void PlayWardrobeSequence(float value)
    {
        if (value >= 0.4 && !wardrobeActioned)
        {
            Debug.Log("Wardrobe actioned, starting coroutine");
            wardrobeActioned = true;

            StartCoroutine(wardrobeSequence.WardrobeCoroutine());
        }
    }

    public void ChangeScene(float value)
    {
        if(sceneChanged) return;
        
        if (value > 0.7 || value < 0.3)
        {
            sceneChanged = true;
            StartCoroutine(sceneTransition.ChangeSceneRoutine(finalScene));
        }
    }
}
