using UnityEngine;

public class Inimigo : MonoBehaviour
{
    // --- CONFIGURAÇÕES DO INIMIGO ---
    public Transform Player;
    public float DistanciaAtaque = 0f;
    public float IntervaloAtaque = 0f;
    public bool eBruxaBoss = false;      // No Sapo: False | Na Bruxa: True

    // --- REFERÊNCIAS DE OBJETOS ---
    public GameObject SpriteAtaque;
    public GameObject Poderes;
    public Transform FirePoint;

    // --- VARIÁVEIS INTERNAS (CACHE) ---
    private float cronometro;
    private Animator anim;

    void Start()
    {
        // 1. Cache do Animator
        if (SpriteAtaque != null)
            anim = SpriteAtaque.GetComponent<Animator>();

        // 2. Busca automática do Player pela Tag
        if (Player == null)
        {
            GameObject PlayerObjeto = GameObject.FindWithTag("Player");
            if (PlayerObjeto != null)
            {
                Player = PlayerObjeto.transform; // AQUI: Atribuímos o transform encontrado à variável player
            }
        }

        // 3. Inicializa o cronómetro
        cronometro = IntervaloAtaque;
    }

    void Update()
    {
        // Segurança: Se não houver player ou animator, o código não corre
        if (Player == null || anim == null) return;

        // --- LÓGICA DE DISTÂNCIA E ATAQUE ---
        float distancia = Vector2.Distance(transform.position, Player.position);

        if (distancia <= DistanciaAtaque)
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
            // Se o player estiver longe, o inimigo fica em Idle
            anim.Play("idle");
        }
    }

    // --- MÉTODO DE ATAQUE ---
    void Atacar()
    {
        anim.Play("ataque");

        if (FirePoint == null || Poderes == null) return;

        // Instancia a magia
        GameObject PoderesObjeto = Instantiate(Poderes, FirePoint.position, FirePoint.rotation);

        // Obtém o script de magia do objeto criado
        Poderes scriptPoderes = PoderesObjeto.GetComponent<Poderes>();

        if (scriptPoderes != null)
        {
            // Define o comportamento da magia com base na variável eBruxaBoss
            // Se for sapo (false), a magia segue a lógica de linha reta do seu script magia
            scriptPoderes.eDaBruxa = eBruxaBoss;
        }
    }
}