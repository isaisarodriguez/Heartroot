using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogoBruxa : MonoBehaviour
{
    // --- REFERÊNCIAS DE UI ---
    public GameObject PainelDialogo;
    public TextMeshProUGUI TextoDialogo;

    // --- VARIÁVEIS DE CONTROLO ---
    private string[] frases;
    private int index = 0;
    private bool EmDialogo = false;

    // --- SISTEMA DE DIÁLOGO ---

    public void IniciarDialogo()
    {
        if (EmDialogo) return;
        EmDialogo = true;

        // Procura o sistema de missões para decidir o que a Bruxa vai dizer
        Missoes M = Object.FindAnyObjectByType<Missoes>();

        if (M != null && M.TemDiario)
        {
            frases = new string[] {
                "Bruxa: Oh! Vejo que já recuperaste o meu diário!",
                "Bruxa: Podes seguir caminho, não te incomodarei mais."
            };
        }
        else
        {
            frases = new string[] {
                "Bruxa: Espera! Não me destruas...",
                "Bruxa: Desculpa, eu só quero o meu diário de volta, os sapos roubaram-no.",
                "Bruxa: Ajuda-me a recupera-lo, pelo visto és mais forte do que eu."
            };

            // Se não tem o diário, aceita automaticamente a missão
            if (M != null) M.AceitarMissao("Recuperar Diário");
        }

        index = 0;
        PainelDialogo.SetActive(true);
        BloquearPlayer(true); // Congela a Maia durante a conversa
        MostrarFraseAtual();
    }

    public void ProximaFrase()
    {
        if (index < frases.Length - 1)
        {
            index++;
            MostrarFraseAtual();
        }
        else
        {
            TerminarTudo();
        }
    }

    void MostrarFraseAtual() => TextoDialogo.text = frases[index];

    void TerminarTudo()
    {
        PainelDialogo.SetActive(false);
        EmDialogo = false;
        BloquearPlayer(false); // Devolve o controlo ao jogador
    }

    // --- INTERAÇÃO COM O MUNDO ---

    void BloquearPlayer(bool bloquear)
    {
        // Encontra os scripts de movimento e ataque para os desligar/ligar
        Player p = Object.FindAnyObjectByType<Player>();
        Ataque a = Object.FindAnyObjectByType<Ataque>();

        if (p != null) p.enabled = !bloquear;
        if (a != null) a.enabled = !bloquear;
    }

    void Update()
    {
        // Se já está a falar, não precisa de detetar proximidade
        if (EmDialogo) return;

        GameObject PlayerObjeto = GameObject.FindWithTag("Player");
        if (PlayerObjeto == null) return;

        // Se a Maia estiver perto (distância < 4) e carregar no 'E'
        if (Vector2.Distance(transform.position, PlayerObjeto.transform.position) < 4f)
        {
            bool apertouE = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) || Input.GetKeyDown(KeyCode.E);

            if (apertouE)
            {
                IniciarDialogo();
            }
        }
    }
}