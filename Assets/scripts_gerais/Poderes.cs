using UnityEngine;

public class Poderes : MonoBehaviour
{
    public float velocidade = 10f;
    public float dano = 10f;
    public bool eDaBruxa = false;
    public bool disparadaPorInimigo = false; // Define se o dano é para a Maia ou Inimigos

    private Transform alvoMaia;

    void Start()
    {
        if (eDaBruxa)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) alvoMaia = p.transform;
        }
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (eDaBruxa && alvoMaia != null)
            transform.position = Vector2.MoveTowards(transform.position, alvoMaia.position, velocidade * Time.deltaTime);
        else
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // SE FOI INIMIGO QUE DISPAROU -> SÓ ATACA PLAYER
        if (disparadaPorInimigo && collision.CompareTag("Player"))
        {
            CausarDano(collision);
        }
        // SE FOI A MAIA QUE DISPAROU -> SÓ ATACA INIMIGO
        else if (!disparadaPorInimigo && collision.CompareTag("Inimigo"))
        {
            CausarDano(collision);
        }
    }

    void CausarDano(Collider2D outro)
    {
        if (outro.TryGetComponent<Vida>(out Vida scriptVida))
        {
            scriptVida.ReceberDano(dano);
        }
        Destroy(gameObject);
    }
}