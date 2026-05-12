using UnityEngine;
using System.Collections;

public class Vida : MonoBehaviour
{
    // --- ATRIBUTOS DE VIDA ---
    public float VidaMax = 100f;
    private float VidaAtual;

    // --- ESTADOS E TEMPOS ---
    public bool eBruxa = false;
    public float Cooldown = 0.5f;
    private bool PodeReceberDano = true;

    // --- COMPONENTES ---
    public Animator anim;

    void Start()
    {
        // Inicializa a vida
        VidaAtual = VidaMax;

        // Cache do Animator (procura no objeto ou nos "filhos" se estiver vazio)
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    // --- FUNÇÕES PÚBLICAS ---

    public void ReceberDano(float dano)
    {
        // O "Guarda": Se estiver invencível, ignora o dano
        if (!PodeReceberDano) return;

        VidaAtual -= dano;

        // Feedback visual de dano
        if (anim != null)
            anim.Play("dano");

        // Verifica morte
        if (VidaAtual <= 0)
        {
            if (eBruxa)
                FinalizarLutaBruxa();
            else
                Destroy(gameObject);
        }
        else
        {
            // Se não morreu, fica invencível por um curto período
            StartCoroutine(ResetarDano());
        }
    }

    // --- LÓGICA ESPECÍFICA DE BOSS ---

    void FinalizarLutaBruxa()
    {
        PodeReceberDano = false;

        // 1. Para de atacar
        Inimigo scriptInimigo = GetComponent<Inimigo>();
        if (scriptInimigo != null)
            scriptInimigo.enabled = false;

        // 2. Visual: Idle
        if (anim != null)
            anim.Play("idle");

        // --- NOVA LINHA AQUI: LIBERTAR A BARREIRA DO PLAYER ---
        Player p = Object.FindAnyObjectByType<Player>();
        if (p != null)
        {
            p.PodePassar = true; // Agora ela pode passar assim que a bruxa para de lutar
        }
        // ------------------------------------------------------

        // 3. História: Iniciar Diálogo
        DialogoBruxa scriptDialogo = Object.FindAnyObjectByType<DialogoBruxa>();
        if (scriptDialogo != null)
        {
            scriptDialogo.IniciarDialogo();
        }
        else
        {
            Debug.LogError("O script DialogoBruxa não foi encontrado na cena!");
        }
    }

    // --- COROUTINES ---

    IEnumerator ResetarDano()
    {
        PodeReceberDano = false;
        yield return new WaitForSeconds(Cooldown);
        PodeReceberDano = true;
    }
}