
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionChatController : MonoBehaviour
{
    public TMP_InputField chatInput;
    public Button sendButton;
    public DialogueUI dialogueUI;
    public QwenClient qwenClient;

    void Start()
    {
        sendButton.onClick.AddListener(SendMessageToCompanion);
    }

    public void SendMessageToCompanion()
    {
        string message = chatInput.text.Trim();

        if (string.IsNullOrEmpty(message))
            return;

        chatInput.text = "";

        // Clear the old response and prepare for incoming text.
        dialogueUI.BeginStreaming("COMPANION");

        sendButton.interactable = false;

        qwenClient.SendMessageToQwen(
            message,
            dialogueUI.AppendStreamingText,
            OnReplyComplete,
            OnReplyError
        );
    }

    void OnReplyComplete()
    {
        dialogueUI.ScheduleClear();

        sendButton.interactable = true;
        chatInput.ActivateInputField();
    }

    void OnReplyError(string error)
    {
        Debug.LogError("Qwen streaming error: " + error);

        dialogueUI.ShowDialogue(
            "COMPANION",
            "Hmm, something went wrong while I was thinking. Try again?"
        );

        sendButton.interactable = true;
        chatInput.ActivateInputField();
    }
}
