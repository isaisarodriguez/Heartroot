using UnityEngine;
using System.Collections;

public class PopUpManager : MonoBehaviour
{
    // Arraste o objeto "Image" (o fundo que ocupa o ecrã) para aqui no Inspector
    public GameObject painelCompleto;
    public float tempoDeEspera = 3f;

    void Start()
    {
        // Garante que o pop-up começa desligado
        painelCompleto.SetActive(false);
    }

    public void MostrarPopUp()
    {
        StartCoroutine(RotinaPopUp());
    }

    IEnumerator RotinaPopUp()
    {
        painelCompleto.SetActive(true);
        yield return new WaitForSeconds(tempoDeEspera);
        painelCompleto.SetActive(false);
    }
}
