
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text speakerName;
    public TMP_Text dialogueText;
    public DialogueBoxResizer boxResizer;

    [Header("Auto Clear")]
    public float clearDelay = 10f;

    private Coroutine clearCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string speaker, string message)
    {
        CancelClearTimer();

        dialoguePanel.SetActive(true);
        speakerName.text = speaker;
        dialogueText.text = message;
        dialogueText.maxVisibleCharacters = int.MaxValue;

        ResizeDialogue();
    }

    public void BeginStreaming(string speaker)
    {
        CancelClearTimer();

        dialoguePanel.SetActive(true);
        speakerName.text = speaker;

        // Start a fresh reply instead of keeping the old chat history.
        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = int.MaxValue;

        ResizeDialogue();
    }

    public void AppendStreamingText(string chunk)
    {
        dialogueText.text += chunk;
        dialogueText.maxVisibleCharacters = int.MaxValue;

        ResizeDialogue();
    }

    // Call this only after the complete response has streamed.
    public void ScheduleClear()
    {
        CancelClearTimer();
        clearCoroutine = StartCoroutine(ClearAfterDelay());
    }

    IEnumerator ClearAfterDelay()
    {
        yield return new WaitForSeconds(clearDelay);

        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = int.MaxValue;

        ResizeDialogue();
        clearCoroutine = null;
    }

    void CancelClearTimer()
    {
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
            clearCoroutine = null;
        }
    }

    void ResizeDialogue()
    {
        dialogueText.ForceMeshUpdate();

        if (boxResizer != null)
            boxResizer.Resize();
    }

    public void HideDialogue()
    {
        CancelClearTimer();
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
    }
}
