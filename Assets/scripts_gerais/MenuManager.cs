using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Tem de ter o "public" para aparecer no Unity!
    public void ReiniciarJogo()
    {
        // Se pausaste o jogo na morte, tens de despausar aqui
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}