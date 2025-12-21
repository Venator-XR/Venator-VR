using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorEscenas : MonoBehaviour
{
    public void JugarOtraVez()
    {
        Time.timeScale = 1; // Importante: Quitar la pausa antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void SalirDelJuegoTotal()
    {
        Application.Quit(); // Esto cierra el .exe o la app (no funciona en el editor)
        Debug.Log("Saliendo del juego...");
    }
}