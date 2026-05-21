using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // --- VARIÁVEIS ---
    public GameObject SpritePlayer;
    public float Velocidade = 0f;
    public float limiteX = 0f;
    public bool PodePassar = false;
    

    // Variável para guardar o Animator e não ter de o repetir sempre
    private Animator animator;

    void Start()
    {
        // Atribuímos o componente no início para o Update poder usá-lo
        animator = SpritePlayer.GetComponent<Animator>();
        
}

    void Update()
    { 

        // Definição das informações de estado da animação
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        // --- BLOCO 1: CONTROLO DE DANO ---
        // Se a animação de dano ainda estiver a tocar, bloqueia outros movimentos
        if (stateInfo.IsName("dano") && stateInfo.normalizedTime < 1.0f)
        {
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

        // --- BLOCO 3: MOVIMENTAÇÃO E ANIMAÇÕES DE DIREÇÃO ---

        // Esquerda
        //if (Keyboard.current.aKey.wasPressedThisFrame)
        // {
        //   animator.Play("esq");
        // }
        //  if (Keyboard.current.aKey.isPressed)
        //  {
        //   this.transform.Translate(-Velocidade * Time.deltaTime, 0, 0);
        // }

        // Direita
        // if (Keyboard.current.dKey.wasPressedThisFrame)
        //{
        // animator.Play("dir");
        // }
        // if (Keyboard.current.dKey.isPressed)
        // {
        // this.transform.Translate(Velocidade * Time.deltaTime, 0, 0);
        // }

        // Cima
        //   if (Keyboard.current.wKey.wasPressedThisFrame)
        //  {
        //  animator.Play("cima");
        // }
        //  if (Keyboard.current.wKey.isPressed)
        //  {
        //  this.transform.Translate(0, Velocidade * Time.deltaTime, 0);
        // }

        // Baixo
        //  if (Keyboard.current.sKey.wasPressedThisFrame)
        //  {
        //   animator.Play("baixo");
        //}
        // if (Keyboard.current.sKey.isPressed)
        //  {
        //  this.transform.Translate(0, -Velocidade * Time.deltaTime, 0);
        // }

        // --- BLOCO 4: LIMITES DO CENÁRIO ---
        if (!PodePassar) // O "!" significa NÃO. Ou seja, se a permissão for falsa.
        {
            if (transform.position.x < limiteX)
            {
                transform.position = new Vector3(limiteX, transform.position.y, transform.position.z);
            }
        }
    }

}    

