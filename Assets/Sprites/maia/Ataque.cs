using UnityEngine;
using UnityEngine.InputSystem;

public class Ataque : MonoBehaviour
{
    // --- REFERÊNCIAS DE OBJETOS ---
    public GameObject SpriteAtaque;
    public GameObject Poderes;
    public Transform FirePoint;

    // --- VARIÁVEIS INTERNAS ---
    private Animator anim;

    void Start()
    {
        // Guardamos o Animator no início para não ter de o procurar em cada frame
        anim = SpriteAtaque.GetComponent<Animator>();
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
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayAtaque();
        }
    }

    // --- LÓGICA DE COMBATE ---
    void PlayAtaque()
    {
        // Ativa a animação de ataque
        anim.Play("ataque");

        // Cria o projétil (Poderes) na posição e rotação do FirePoint
        GameObject PoderesObjeto = Instantiate(Poderes, FirePoint.position, FirePoint.rotation);

        // Acede ao script 'magia' do objeto criado e define que não é da bruxa
        PoderesObjeto.GetComponent<Poderes>().eDaBruxa = false;
    }
}