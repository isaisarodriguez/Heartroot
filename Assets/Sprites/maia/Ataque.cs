using UnityEngine;
using UnityEngine.InputSystem;

public class Ataque : MonoBehaviour
{
    public GameObject SpriteAtaque;
    public GameObject Poderes;
    public Transform FirePoint;

    [Header("Configurações de Áudio")]
    public AudioSource audioSource;
    public AudioClip somAtaque;

    private Animator anim;

    void Start()
    {
        anim = SpriteAtaque.GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("dano"))
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlayAtaque();
        }
    }

    void PlayAtaque()
    {
        anim.Play("ataque");

        if (audioSource != null && somAtaque != null)
        {
            audioSource.PlayOneShot(somAtaque);
        }

        GameObject PoderesObjeto = Instantiate(Poderes, FirePoint.position, FirePoint.rotation);

        if (PoderesObjeto.GetComponent<Poderes>() != null)
        {
            PoderesObjeto.GetComponent<Poderes>().eDaBruxa = false;
        }
    }
}