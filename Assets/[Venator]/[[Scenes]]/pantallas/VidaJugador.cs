using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int vidas = 2;

    [Header("Interfaces (Arrastra los Canvas)")]
    public GameObject panelDano;
    public GameObject canvasDerrota;

    private bool haMuerto = false;
    private Coroutine corrutinaParpadeo;
    private CanvasGroup panelDanoCG;

    private void Start()
    {
        if (panelDano != null)
        {
            panelDanoCG = panelDano.GetComponent<CanvasGroup>();
            if (panelDanoCG == null) panelDanoCG = panelDano.AddComponent<CanvasGroup>();

            panelDanoCG.alpha = 0;
            panelDano.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemigo") && !haMuerto) RecibirDano();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") && !haMuerto) RecibirDano();
    }

    void RecibirDano()
    {
        vidas--;

        // BUSCAMOS AL VAMPIRO PARA RALENTIZARLO
        Unity.AI.Navigation.Samples.agentFollowPos scriptVampiro = FindFirstObjectByType<Unity.AI.Navigation.Samples.agentFollowPos>();
        if (scriptVampiro != null)
        {
            scriptVampiro.RalentizarVampiro(10f); // 10 segundos de lentitud
        }

        if (vidas == 1)
        {
            // ... (aquí va tu código del parpadeo de sangre que ya tenías)
            if (panelDano != null)
            {
                panelDano.SetActive(true);
                if (corrutinaParpadeo != null) StopCoroutine(corrutinaParpadeo);
                corrutinaParpadeo = StartCoroutine(HacerParpadearSangre());
            }
        }
        else if (vidas <= 0)
        {
            Morir();
        }
    }

    IEnumerator HacerParpadearSangre()
    {
        float velocidad = 3f;
        while (vidas == 1 && !haMuerto)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * velocidad));
            float alphaAjustado = 0.2f + (alpha * 0.5f);
            if (panelDanoCG != null) panelDanoCG.alpha = alphaAjustado;
            yield return null;
        }
    }

    void Morir()
    {
        haMuerto = true;
        Time.timeScale = 1; // ¡OJO! No pauses el tiempo aquí si quieres cambiar de escena
        SceneManager.LoadScene("Pantalla_Derrota"); // Asegúrate de que el nombre coincida exactamente
    }
}