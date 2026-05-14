using UnityEngine;
using System.Collections;

public class Vida : MonoBehaviour
{
    // --- ATRIBUTOS DE VIDA ---
    public float VidaMax = 100f;
    private float VidaAtual;

    // --- ESTADOS E TEMPOS ---
    public bool eBruxa = false;
    public bool ePlayer = false;
    public float Cooldown = 0.5f;
    private bool PodeReceberDano = true;

    // --- COMPONENTES E UI ---
    public Animator anim;
    public GameObject painelGameOver;

    // --- ÁUDIO (ADICIONADO) ---
    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somMorte;

    void Start()
    {
        VidaAtual = VidaMax;

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        // Tenta encontrar o AudioSource automaticamente se não arrastares
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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
            else if (ePlayer)
            {
                MorrerPlayer();
            }
            else
            {
                Destroy(gameObject);
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
        // 1. Toca o som de morte (antes da pausa)
        if (audioSource != null && somMorte != null)
        {
            // Definimos para ignorar a pausa, caso o som seja longo
            audioSource.ignoreListenerPause = true;
            audioSource.PlayOneShot(somMorte);
        }

        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
            Time.timeScale = 0f; // Pausa o jogo
        }

        if (anim != null)
            anim.Play("morte");

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