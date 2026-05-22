using UnityEngine;

[CreateAssetMenu(fileName = "NovoFicheiroDialogo", menuName = "Sistema Dialogo/NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;          // Nome que vai aparecer na UI
    public Sprite npcPortrait;      // Foto do NPC que vai aparecer na UI
    [Range(0.01f, 0.1f)]
    public float typingSpeed = 0.04f; // Velocidade da escrita (padrão do vídeo)
}
