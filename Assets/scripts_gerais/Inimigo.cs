using UnityEngine;
using UnityEngine.InputSystem; // Necessário para o Keyboard.current

public class Inimigo : MonoBehaviour
{
    // --- CONFIGURAÇÕES DO INIMIGO ---
    public Transform Player;
    public float DistanciaAtaque = 5f;
    public float IntervaloAtaque = 2f;
    public bool eBruxaBoss = false;

    // --- NOVA LÓGICA DE MISSÃO ---
    public float distanciaEntrega = 3f;
    private bool missaoFinalizada = false; // ESTAVA A FALTAR ESTA LINHA!

    // --- REFERÊNCIAS DE OBJETOS ---
    public GameObject SpriteAtaque;
    public GameObject Poderes;
    public Transform FirePoint;

    // --- VARIÁVEIS INTERNAS ---
    private float cronometro;
    private Animator anim;

    void Start()
    {
        if (SpriteAtaque != null)
            anim = SpriteAtaque.GetComponent<Animator>();

        if (Player == null)
        {
            GameObject PlayerObjeto = GameObject.FindWithTag("Player");
            if (PlayerObjeto != null) Player = PlayerObjeto.transform;
        }

        cronometro = IntervaloAtaque;
    }

    void Update()
    {
        if (Player == null || anim == null) return;

        float distancia = Vector2.Distance(transform.position, Player.position);

        // 1. VERIFICAR ENTREGA (Lógica corrigida para usar o script Missoes)
        if (eBruxaBoss && !missaoFinalizada)
        {
            // Procuramos o teu script de missões
            Missoes gestor = Object.FindFirstObjectByType<Missoes>();

            if (gestor != null && gestor.TemDiario && distancia <= distanciaEntrega)
            {
                // Usando o Input System novo para garantir que funciona
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    FinalizarMissao();
                    return;
                }
            }
        }

        // 2. LÓGICA DE ATAQUE
        if (!missaoFinalizada && distancia <= DistanciaAtaque)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= IntervaloAtaque)
            {
                Atacar();
                cronometro = 0;
            }
        }
        else
        {
            // Só fica em Idle se não estiver a atacar e a missão não tiver acabado de acabar
            if (!missaoFinalizada) anim.Play("idle");
        }
    }

    void Atacar()
    {
        anim.Play("ataque");
        if (FirePoint == null || Poderes == null) return;

        GameObject PoderesObjeto = Instantiate(Poderes, FirePoint.position, FirePoint.rotation);
        Poderes scriptPoderes = PoderesObjeto.GetComponent<Poderes>();

        if (scriptPoderes != null)
        {
            scriptPoderes.eDaBruxa = eBruxaBoss;
        }
    }

    // ESTE MÉTODO ESTAVA A FALTAR NO TEU SCRIPT!
    void FinalizarMissao()
    {
        missaoFinalizada = true;
        anim.Play("idle");

        Debug.Log("Missão Concluída! Ativando Pop-up...");

        // Chama o Pop-up
        PopUpManager popup = Object.FindFirstObjectByType<PopUpManager>();
        if (popup != null)
        {
            popup.MostrarPopUp();
        }
    }
}