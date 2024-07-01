using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    [SerializeField] private TMP_InputField dialoguePlayerResponse;

    private float typingTime = 0.05f;
    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    private string respuesta;
    private string npcLine;
    private NpcApiResponse npc;
    private bool seguir = false;

    async void Start()
    {
        // Suscribe el método HandleSubmit al evento onSubmit del TMP_InputField
        dialoguePlayerResponse.onSubmit.AddListener(HandleSubmit);

        // Crear el NPC en la base de datos
        npc = new NpcApiResponse
        {
            name = "Fosy",
            mood = 1,
            context = "Youre a lost fox who doesnt know how to get there. Youre strong and kind",
            flow = new List<FlowItem>()
        };

        npc.flow.Add(new FlowItem { key = true, value = "Where are we?" });

        // Llamada al servicio para crear el NPC
        npc = await npcService.CreateNpc(npc);
        if (npc != null)
        {
            Debug.Log($"NPC creado con ID: {npc.id}");
        }
        else
        {
            Debug.Log("Error al crear el NPC.");
        }
        Debug.Log(npc.id);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetButtonDown("Fire1"))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            else if (dialogueText.text == npcLine && seguir)
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = npcLine;
            }
        }
    }

    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        npcLine = npc.flow[npc.flow.Count - 1].value;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        seguir = false;
        lineIndex++;
        if (lineIndex < 3)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            dialoguePanel.SetActive(false);
            dialogueMark.SetActive(true);
            Time.timeScale = 1f;
            didDialogueStart = false;
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in npcLine)
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialogueMark.SetActive(true);
            //Debug.Log("en el area");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);
            //Debug.Log("fuera");
        }
    }

    // Método para manejar el submit del TMP_InputField
    private async void HandleSubmit(string input)
    {
        respuesta = input;
        Debug.Log("Respuesta del jugador: " + respuesta);
        npc = await npcService.SendMessage(npc.id, respuesta);
        if (npc != null)
        {
            int last = npc.flow.Count;
            Debug.Log($"Mensaje: {npc.flow[last - 1].value}");
            npcLine = npc.flow[last - 1].value;
        }
        else
        {
            Debug.Log("No se recibió ninguna respuesta.");
            npcLine = "no lo se viejo :/";
        }
        seguir = true;
    }
}
