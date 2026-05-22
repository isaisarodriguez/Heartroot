using UnityEngine;
using UnityEngine.InputSystem;

public class Ataque : MonoBehaviour
{
    // --- REFERÊNCIAS DE OBJETOS ---
    public GameObject SpriteAtaque;
    public GameObject Poderes;
    public Transform FirePoint;

    [Header("Configurações de Áudio")]
    public AudioSource audioSource; // Arrastar o componente Audio Source do Player para aqui
    public AudioClip somAtaque;      // Arrastar o ficheiro de som (.wav) para aqui

    // --- VARIÁVEIS INTERNAS ---
    private Animator anim;

    void Start()
    {
        // Guardamos o Animator no início para não ter de o procurar em cada frame
        anim = SpriteAtaque.GetComponent<Animator>();

        // Verificação de segurança caso te esqueças de arrastar o AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 1. VERIFICAÇÃO DE ESTADO (BLOQUEIO)
        // Se estiver a levar dano, sai do Update e não deixa atacar
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("dano"))
        {
            return;
        }

        // 2. INPUT DO JOGADOR
        // Verifica se a tecla Espaço foi pressionada neste frame
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlayAtaque();
        }
    }

    // --- LÓGICA DE COMBATE ---
    void PlayAtaque()
    {
        // Ativa a animação de ataque
        anim.Play("ataque");

        // --- NOVO: Toca o som do ataque ---
        if (audioSource != null && somAtaque != null)
        {
            audioSource.PlayOneShot(somAtaque);
        }

        // Cria o projétil (Poderes) na posição e rotação do FirePoint
        GameObject PoderesObjeto = Instantiate(Poderes, FirePoint.position, FirePoint.rotation);

        // Acede ao script 'magia' do objeto criado e define que não é da bruxa
        // Nota: Garante que o componente se chama 'Poderes' (como o prefab) ou 'magia'
        if (PoderesObjeto.GetComponent<Poderes>() != null)
        {
            PoderesObjeto.GetComponent<Poderes>().eDaBruxa = false;
        }
    }
}