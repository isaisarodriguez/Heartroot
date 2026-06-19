using UnityEngine;
using UnityEngine.InputSystem; // Importante para detetar o rato

public class PopUpManager : MonoBehaviour
{
    // Arraste o teu Quadro de Diálogo para aqui no Inspector
    public GameObject painelCompleto;
    private bool dialogoAtivo = false;

    void Start()
    {
        // Garante que o pop-up começa desligado
        if (painelCompleto != null)
            painelCompleto.SetActive(false);
    }

    void Update()
    {
        // Se o diálogo estiver aberto e o jogador der Clique Esquerdo no rato, fecha o diálogo
        if (dialogoAtivo && Mouse.current.leftButton.wasPressedThisFrame)
        {
            FecharPopUp();
        }
    }

    public void MostrarPopUp()
    {
        if (painelCompleto != null)
        {
            painelCompleto.SetActive(true);
            dialogoAtivo = true;
            Time.timeScale = 0f; // TRAVA O JOGO (Congela inimigos e o tempo)
            Debug.Log("[DIÁLOGO] Diálogo aberto e jogo pausado.");
        }
    }

    public void FecharPopUp()
    {
        if (painelCompleto != null)
        {
            painelCompleto.SetActive(false);
            dialogoAtivo = false;
            Time.timeScale = 1f; // DESPAUSA O JOGO (Tudo volta ao normal)
            Debug.Log("[DIÁLOGO] Diálogo fechado e jogo retomado.");
        }
    }
}