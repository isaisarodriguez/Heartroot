using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [Header("Componentes de UI")]
    [SerializeField] private Slider sliderBarraVida;

    [Header("Referência ao Player")]
    [SerializeField] private Vida scriptVidaPlayer;

    private void OnEnable()
    {
        if (scriptVidaPlayer != null)
        {
            scriptVidaPlayer.OnVidaMudou += AtualizarBarra;

            // --- ESTA LINHA RESOLVE O PROBLEMA ---
            // Força a barra a começar cheia assim que o HUD aparece na tela
            AtualizarBarra(100f, 100f);
        }
    }

    private void OnDisable()
    {
        if (scriptVidaPlayer != null)
        {
            scriptVidaPlayer.OnVidaMudou -= AtualizarBarra;
        }
    }

    private void AtualizarBarra(float atual, float maxima)
    {
        if (sliderBarraVida != null)
        {
            sliderBarraVida.value = atual / maxima;
        }
    }
}