using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public static Fader Instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] CinemachineCamera vcam;

    // Esta variável vai deixar de dar erro assim que adicionares o "using Unity.Cinemachine;" no topo
    CinemachinePositionComposer transposer;
    Vector3 originalDamping;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (vcam != null)
        {
            transposer = vcam.GetComponent<CinemachinePositionComposer>();
        }

        if (transposer != null)
        {
            originalDamping = transposer.Damping;
        }
    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha, t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / fadeDuration);
            await Task.Yield();
        }
        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        await Fade(1);
        SetDamping(Vector3.zero);
    }
    public async Task FadeIn()
    {
        await Fade(0);
        SetDamping(originalDamping);
    }

    void SetDamping(Vector3 d)
    {
        if (!transposer) return;
        transposer.Damping = d;
    }
}
