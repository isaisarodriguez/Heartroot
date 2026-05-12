using UnityEngine;
using System.Collections;

public class Vida : MonoBehaviour
{
    // --- ATRIBUTOS DE VIDA ---
    public float VidaMax = 100f;
    private float VidaAtual;

    // --- ESTADOS E TEMPOS ---
    public bool eBruxa = false;
    public bool ePlayer = false; // NOVA VARIÁVEL: Marca como True no Inspector da Maia!
    public float Cooldown = 0.5f;
    private bool PodeReceberDano = true;

    // --- COMPONENTES E UI ---
    public Animator anim;
    public GameObject painelGameOver; // NOVA VARIÁVEL: Arrastar o PainelGameOver para aqui

    void Start()
    {
        VidaAtual = VidaMax;

        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    public void ReceberDano(float dano)
    {
        if (!PodeReceberDano) return;

        VidaAtual -= dano;

        if (anim != null)
            anim.Play("dano");

        if (VidaAtual <= 0)
        {
            if (eBruxa)
            {
                FinalizarLutaBruxa();
            }
            else if (ePlayer) // SE FOR A MAIA QUE MORREU
            {
                MorrerPlayer();
            }
            else
            {
                Destroy(gameObject); // Inimigos normais (sapos) apenas desaparecem
            }
        }
        else
        {
            StartCoroutine(ResetarDano());
        }
    }

    // --- LÓGICA DE DERROTA DO PLAYER ---
    void MorrerPlayer()
    {
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true); // Faz o botão aparecer
            Time.timeScale = 0f;            // Pausa o jogo para dar efeito de morte
        }

        if (anim != null)
            anim.Play("morte"); // Se tiveres uma animação de morte, ela toca aqui

        Debug.Log("A Maia morreu. Game Over!");
    }

    // --- LÓGICA ESPECÍFICA DE BOSS ---
    void FinalizarLutaBruxa()
    {
        PodeReceberDano = false;

        Inimigo scriptInimigo = GetComponent<Inimigo>();
        if (scriptInimigo != null)
            scriptInimigo.enabled = false;

        if (anim != null)
            anim.Play("idle");

        Player p = Object.FindAnyObjectByType<Player>();
        if (p != null)
        {
            p.PodePassar = true;
        }

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

    IEnumerator ResetarDano()
    {
        PodeReceberDano = false;
        yield return new WaitForSeconds(Cooldown);
        PodeReceberDano = true;
    }
}