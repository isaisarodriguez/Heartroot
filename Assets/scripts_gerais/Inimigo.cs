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
    public NPCDialogue dialogoAgradecimento;


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

        // LÓGICA DE ATAQUE (Só ataca se a missão não tiver sido finalizada)
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

        // 1. Entrega e conclui a missão no caderno roxo
        string questIDDaBruxa = "DiarioBruxa";
        if (QuestController.Instance != null)
        {
            QuestController.Instance.HandInQuest(questIDDaBruxa);
        }

        // 2. Chamar o teu InventoryController para limpar o Diário da mala!
        if (InventoryController.Instance != null)
        {
            // ID 13 (o Diário) e quantidade 1
            InventoryController.Instance.RemoveItemsFromInventory(13, 1);
            Debug.Log("[MISSÃO] Sinal enviado para remover o Diário do inventário.");
        }

        // 3. Ativa o diálogo de agradecimento na Bruxa
        DialogoBruxa scriptDialogo = GetComponent<DialogoBruxa>();
        if (scriptDialogo != null && dialogoAgradecimento != null)
        {
            scriptDialogo.AtivarDialogo(dialogoAgradecimento);
        }
    }
}