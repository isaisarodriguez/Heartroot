using UnityEngine;
using TMPro;
using UnityEngine.UI; // NECESSÁRIO para usar o componente Image do retrato
using UnityEngine.InputSystem;
using System.Collections; // NECESSÁRIO para usar as Coroutines (efeito de escrever letra a letra)

public class DialogoBruxa : MonoBehaviour
{
    // --- REFERÊNCIAS DE UI (Vindo do Vídeo + Teu) ---
    public GameObject PainelDialogo;
    public TextMeshProUGUI TextoDialogo;
    public TextMeshProUGUI NomeTexto;      // Novo: Para mostrar o nome de quem fala
    public Image RetratoImagem;           // Novo: Para mostrar a foto do NPC

    // --- REFERÊNCIA DO SCRIPTABLE OBJECT (Do Vídeo) ---
    public NPCDialogue DadosDialogo; // Arrasta o ficheiro de diálogo criado aqui

    // --- VARIÁVEIS DE CONTROLO ---
    private string[] frases;
    private int index = 0;
    private bool EmDialogo = false;
    private bool IsTyping = false;       // Novo: Controla se o texto ainda está a ser escrito
    private Coroutine typingCoroutine;   // Novo: Guarda a rotina de escrita para a podermos parar

    // --- SISTEMA DE DIÁLOGO ---

    public void IniciarDialogo()
    {
        if (EmDialogo) return;
        EmDialogo = true;

        // Procura o sistema de missões para decidir as falas (Lógica original tua)
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

            if (M != null) M.AceitarMissao("Recuperar Diário");
        }

        index = 0;
        PainelDialogo.SetActive(true);
        BloquearPlayer(true); // Congela a Maia

        // Configura os dados visuais baseados no Scriptable Object do vídeo
        if (DadosDialogo != null)
        {
            NomeTexto.text = DadosDialogo.npcName;
            RetratoImagem.sprite = DadosDialogo.npcPortrait;
        }

        // Inicia a escrita da primeira frase com o efeito letra a letra do vídeo
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine());
    }

    // Efeito Letra a Letra (Adaptado do Minuto 09:16 do vídeo)
    private IEnumerator TypeLine()
    {
        IsTyping = true;
        TextoDialogo.text = ""; // Limpa o texto anterior

        // Se tiveres o Scriptable Object usa a velocidade dele, se não usa 0.04s por padrão
        float velocidadeEscrita = (DadosDialogo != null) ? DadosDialogo.typingSpeed : 0.04f;

        foreach (char letra in frases[index].ToCharArray())
        {
            TextoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadeEscrita);
        }

        IsTyping = false;
    }

    // Avança ou completa o texto instantaneamente se o jogador carregar no 'E' (Minuto 11:35 do vídeo)
    public void ProximaFrase()
    {
        if (IsTyping)
        {
            // Se ainda está a escrever, para o efeito e mostra a frase toda de uma vez
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            TextoDialogo.text = frases[index];
            IsTyping = false;
        }
        else if (index < frases.Length - 1)
        {
            // Se já acabou de escrever e há mais falas, avança para a próxima
            index++;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            // Se não houver mais falas, fecha tudo
            TerminarTudo();
        }
    }

    void TerminarTudo()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        PainelDialogo.SetActive(false);
        EmDialogo = false;
        BloquearPlayer(false); // Devolve o controlo à Maia
    }

    // --- INTERAÇÃO COM O MUNDO (Tua lógica com correções de física) ---

    void BloquearPlayer(bool bloquear)
    {
        Player p = Object.FindAnyObjectByType<Player>();
        Ataque a = Object.FindAnyObjectByType<Ataque>();

        if (p != null)
        {
            p.enabled = !bloquear;

            Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
            if (rb == null) rb = p.GetComponentInParent<Rigidbody2D>();

            if (rb != null && bloquear)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Animator anim = p.GetComponentInChildren<Animator>();
            if (anim != null && bloquear)
            {
                anim.Play("idle");
            }
        }

        if (a != null) a.enabled = !bloquear;
    }

    void Update()
    {
        GameObject PlayerObjeto = GameObject.FindWithTag("Player");
        if (PlayerObjeto == null) return;

        if (Vector2.Distance(transform.position, PlayerObjeto.transform.position) < 4f)
        {
            bool apertouE = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) || Input.GetKeyDown(KeyCode.E);

            if (apertouE)
            {
                if (!EmDialogo)
                {
                    IniciarDialogo();
                }
                else
                {
                    // Se já estiver em diálogo, o botão 'E' serve para passar à próxima frase ou autocompletar
                    ProximaFrase();
                }
            }
        }
    }
}