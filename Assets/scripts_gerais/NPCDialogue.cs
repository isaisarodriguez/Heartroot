using UnityEngine;

[CreateAssetMenu(fileName = "NovoFicheiroDialogo", menuName = "Sistema Dialogo/NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    [Range(0.01f, 0.1f)] public float typingSpeed = 0.04f;
    [TextArea(3, 5)] public string[] frases;

    public int questInProgressIndex;
    public int questCompletedIndex;
    public Quest quest;
}