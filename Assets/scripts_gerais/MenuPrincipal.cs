using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Painéis de UI")]
    [SerializeField] private GameObject telaInicialPanel;

    void Start()
    {
        // 1. Garante que a tela inicial está visível quando o jogo abre
        if (telaInicialPanel != null)
        {
            telaInicialPanel.SetActive(true);
        }

        // 2. Pausa o jogo por trás para nenhum inimigo atacar a Maia no menu
        Time.timeScale = 0f;
    }

    public void ComecarJogo()
    {
        // 1. Esconde a tela inicial para revelar o jogo
        if (telaInicialPanel != null)
        {
            telaInicialPanel.SetActive(false);
        }

        // 2. Despausa o jogo para tudo começar a funcionar!
        Time.timeScale = 1f;

        Debug.Log("[MENU] Jogo iniciado com sucesso!");
    }

    public void FecharJogo()
    {
        Debug.Log("[MENU] O jogador fechou o jogo!");

        // Isto fecha o jogo quando ele for exportado para o computador (.exe)
        Application.Quit();

        // Isto para o botão Play se estiveres a testar dentro do Unity!
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
