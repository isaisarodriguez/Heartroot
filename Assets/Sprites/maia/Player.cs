using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // --- VARIÁVEIS ---
    public GameObject SpritePlayer;
    public float Velocidade = 4f; // Unido com o teu moveSpeed antigo (podes ajustar no Inspector)
    public float limiteX = -20f;  // Mantive o teu valor padrão de -20
    public bool PodePassar = false;

    // --- COMPONENTES E SISTEMA INTERNO ---
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Atribuímos os componentes no início para o Update/FixedUpdate poderem usá-los
        rb = GetComponent<Rigidbody2D>();

        animator = SpritePlayer.GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // Definição das informações de estado da animação
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // --- BLOCO 1: CONTROLO DE DANO ---
        // Se a animação de dano ainda estiver a tocar, bloqueia outros movimentos
        if (stateInfo.IsName("dano") && stateInfo.normalizedTime < 1.0f)
        {
            rb.linearVelocity = Vector2.zero; // Garante que ela para ao levar dano
            return;
        }

        // --- BLOCO 2: VOLTAR AO IDLE ---
        // Se nenhuma tecla estiver pressionada, volta ao idle (sem cortar o dano)
        if (Keyboard.current.anyKey.isPressed == false)
        {
            if (!stateInfo.IsName("idle") && !stateInfo.IsName("dano"))
            {
                animator.Play("idle");
            }
        }

        // --- BLOCO 3: ANIMAÇÕES DE DIREÇÃO ---
        // (Mantivemos o teu sistema original por cliques que disseste que já funciona!)

        // Esquerda
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            animator.Play("esq");
        }

        // Direita
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            animator.Play("dir");
        }

        // Cima
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            animator.Play("cima");
        }

        // Baixo
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            animator.Play("baixo");
        }

        // --- BLOCO 4: LIMITES DO CENÁRIO ---
        if (!PodePassar) // O "!" significa NÃO. Ou seja, se a permissão for falsa.
        {
            if (transform.position.x < limiteX)
            {
                transform.position = new Vector3(limiteX, transform.position.y, transform.position.z);
            }
        }
    }

    // --- MOVIMENTO POR FÍSICA (Substituiu o Translate para evitar bugs) ---
    void FixedUpdate()
    {
        // Lemos as teclas WASD para criar o vetor de movimento
        float moverX = 0f;
        float moverY = 0f;

        if (Keyboard.current.aKey.isPressed) moverX = -1f;
        if (Keyboard.current.dKey.isPressed) moverX = 1f;
        if (Keyboard.current.wKey.isPressed) moverY = 1f;
        if (Keyboard.current.sKey.isPressed) moverY = -1f;

        moveInput = new Vector2(moverX, moverY).normalized;

        // Aplica a velocidade no Rigidbody2D de forma suave
        rb.linearVelocity = moveInput * Velocidade;
    }
}

