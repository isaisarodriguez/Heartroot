using Unity.Cinemachine;
using UnityEngine;

public class mudamapa : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    [SerializeField] float additivePos = 2f;

    enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.gameObject);
        }
    }

    async void FadeTransition(GameObject player)
    {
        // 1. ANTES DO FADE: Congela o movimento do jogador
        ControlarMovimentoPlayer(player, false);

        await Fader.Instance.FadeOut();

        confiner.BoundingShape2D = mapBoundry;

        // No Unity 6, depois de mudar o boundry, limpa o cache para a câmara atualizar o mapa na hora!
        confiner.InvalidateBoundingShapeCache();

        UpdatePlayerPosition(player);

        await Fader.Instance.FadeIn();

        // 2. DEPOIS DO FADE: Devolve o controlo ao jogador
        ControlarMovimentoPlayer(player, true);
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += additivePos;
                break;
            case Direction.Down:
                newPos.y -= additivePos;
                break;
            case Direction.Left:
                newPos.x += additivePos;
                break;
            case Direction.Right:
                newPos.x -= additivePos;
                break;
        }

        player.transform.position = newPos;
    }

    private void ControlarMovimentoPlayer(GameObject playerObj, bool ativar)
    {
        // --- ATUALIZADO: Agora procura pelo componente/script chamado 'Player' ---
        MonoBehaviour scriptMovimento = playerObj.GetComponent("Player") as MonoBehaviour;
        if (scriptMovimento != null)
        {
            scriptMovimento.enabled = ativar; // Desliga ou liga o teu script principal
        }

        // Garante que a física para imediatamente no sítio
        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb != null && !ativar)
        {
            rb.linearVelocity = Vector2.zero; // Unity 6 standard
        }
    }
}
