using UnityEngine;
using System.Collections;
using System; // <-- ADICIONADO para poder usar o Action

public class Vida : MonoBehaviour
{
    // --- ATRIBUTOS DE VIDA ---
    public float VidaMax = 100f;
    private float VidaAtual;

    // --- EVENTO PARA A UI (ADICIONADO) ---
    public event Action<float, float> OnVidaMudou;

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
       
    }

    private void Awake()
    {
        VidaAtual = VidaMax;

        // Atualiza a barra de vida logo no início com o valor máximo
        if (ePlayer) OnVidaMudou?.Invoke(VidaAtual, VidaMax);

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void ReceberDano(float dano)
    {
        if (!PodeReceberDano) return;

        VidaAtual -= dano;

        // SE FOR O PLAYER, avisa a UI para atualizar o Slider!
        if (ePlayer) OnVidaMudou?.Invoke(VidaAtual, VidaMax);

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
        if (audioSource != null && somMorte != null)
        {
            audioSource.ignoreListenerPause = true;
            audioSource.PlayOneShot(somMorte);
        }

        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
            Time.timeScale = 0f;
        }

        if (anim != null)
            anim.Play("morte");

        Debug.Log("A Maia morreu. Game Over!");
    }

    // --- LÓGICA ESPECÍFICA DE BOSS ---
    void FinalizarLutaBruxa()
    {
        Debug.Log("[SISTEMA] Vida da Bruxa chegou a zero! A iniciar FinalizarLutaBruxa()...");

        PodeReceberDano = false;

        Inimigo scriptInimigo = GetComponent<Inimigo>();
        if (scriptInimigo != null)
            scriptInimigo.enabled = false;

        if (anim != null)
            anim.Play("idle");

        Player p = FindAnyObjectByType<Player>();
        if (p != null)
        {
            p.PodePassar = true;
        }

        Debug.Log("[SISTEMA] A procurar o script DialogoBruxa na cena...");

        DialogoBruxa scriptDialogo = FindAnyObjectByType<DialogoBruxa>();
        if (scriptDialogo != null)
        {
            Debug.Log("[SISTEMA] DialogoBruxa encontrado com sucesso! A chamar AtivarDialogoPosDerrota().");
            scriptDialogo.AtivarDialogo(scriptDialogo.dialogoDaBruxa);
        }
        else
        {
            Debug.LogError("[ERRO] O script DialogoBruxa não foi encontrado em NENHUM objeto ativo da cena!");
        }
    }

    IEnumerator ResetarDano()
    {
        PodeReceberDano = false;
        yield return new WaitForSeconds(Cooldown);
        PodeReceberDano = true;
    }
}