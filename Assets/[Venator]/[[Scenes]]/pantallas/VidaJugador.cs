using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para las Corrutinas

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int vidas = 2;

    [Header("Pantallas (Arrastra aquí los Canvas)")]
    public GameObject panelDano;    // El panel rojo de sangre
    public GameObject canvasDerrota; // El Canvas de Game Over

    private bool haMuerto = false;
    private Coroutine corrutinaParpadeo; // Para controlar el parpadeo
    private CanvasGroup panelDanoCG;     // Para controlar la transparencia

    private void Start()
    {
        // 1. Configuramos el CanvasGroup del panel de daño
        if (panelDano != null)
        {
            // Intentamos obtener el componente, si no existe, lo añadimos
            panelDanoCG = panelDano.GetComponent<CanvasGroup>();
            if (panelDanoCG == null)
            {
                panelDanoCG = panelDano.AddComponent<CanvasGroup>();
            }

            // Nos aseguramos de que empiece invisible
            panelDanoCG.alpha = 0;
            panelDano.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemigo") && !haMuerto)
        {
            RecibirDano();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") && !haMuerto)
        {
            RecibirDano();
        }
    }

    void RecibirDano()
    {
        vidas--;

        if (vidas == 1)
        {
            // PRIMER GOLPE: Activamos el parpadeo
            Debug.Log("¡Au! Primer golpe.");
            if (panelDano != null)
            {
                panelDano.SetActive(true); // Lo activamos
                // Iniciamos la corrutina de parpadeo
                if (corrutinaParpadeo != null) StopCoroutine(corrutinaParpadeo);
                corrutinaParpadeo = StartCoroutine(HacerParpadearSangre());
            }
        }
        else if (vidas <= 0)
        {
            // SEGUNDO GOLPE: Fin del juego
            Morir();
        }
    }

    // --- ESTA ES LA NUEVA FUNCIÓN DE PARPADEO ---
    IEnumerator HacerParpadearSangre()
    {
        float velocidad = 3f; // Qué tan rápido parpadea

        while (vidas == 1 && !haMuerto)
        {
            // Usamos Mathf.Sin para crear una onda suave (pulso) entre 0.2 y 0.8 de opacidad
            // Time.time hace que avance, * velocidad lo acelera.
            // Abs lo hace positivo.
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * velocidad));

            // Ajustamos para que no sea totalmente transparente ni totalmente opaco (opcional)
            // Esto hará que oscile entre 0.2 (casi transparente) y 0.7 (bastante rojo)
            float alphaAjustado = 0.2f + (alpha * 0.5f);

            if (panelDanoCG != null)
            {
                panelDanoCG.alpha = alphaAjustado;
            }

            yield return null; // Espera al siguiente frame
        }
    }

    void Morir()
    {
        haMuerto = true;
        Debug.Log("¡Has muerto!");

        // Detenemos el parpadeo si estaba activo
        if (corrutinaParpadeo != null) StopCoroutine(corrutinaParpadeo);

        // 1. Mostrar pantalla de derrota
        if (canvasDerrota != null) canvasDerrota.SetActive(true);

        // 2. Ocultar el panel de daño
        if (panelDano != null) panelDano.SetActive(false);

        // 3. Pausar el tiempo
        Time.timeScale = 0;
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public void JugarOtraVez()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirDelJuego()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    
}