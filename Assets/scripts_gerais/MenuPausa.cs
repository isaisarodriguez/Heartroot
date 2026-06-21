using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    [Header("Painel de Pausa")]
    [SerializeField] private GameObject painelMenuPausa;

    private bool jogoPausado = false;

    void Update()
    {
        // Verifica se o teclado existe e se a tecla ESC foi pressionada
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (jogoPausado)
            {
                RetomarJogo();
            }
            else
            {
                PausarJogo();
            }
        }
    }

    public void PausarJogo()
    {
        if (painelMenuPausa != null)
        {
            painelMenuPausa.SetActive(true); // Mostra o painel
            Time.timeScale = 0f;            // Pausa o tempo do jogo
            jogoPausado = true;
        }
    }

    public void RetomarJogo()
    {
        if (painelMenuPausa != null)
        {
            painelMenuPausa.SetActive(false); // Esconde o painel
            Time.timeScale = 1f;             // Devolve o tempo ao normal
            jogoPausado = false;
        }
    }

    public void FecharJogo()
    {
        Debug.Log("[MENU PAUSA] O jogador fechou o jogo!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}