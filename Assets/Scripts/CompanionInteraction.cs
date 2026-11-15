
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CompanionInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public DialogueUI dialogueUI;
    public TMP_InputField chatInput;

    private Transform player;
    private bool interactionOpen = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError(
                "Player not found! Make sure your player has the 'Player' tag."
            );
        }
    }

    void Update()
    {
        if (player == null || Keyboard.current == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        // Close only when the player walks out of range.
        if (interactionOpen && distance > interactionDistance)
        {
            interactionOpen = false;
            dialogueUI.HideDialogue();
            return;
        }

        // Once open, E must never restart the conversation.
        if (interactionOpen)
            return;

        // Don't open while the player is typing.
        if (chatInput != null && chatInput.isFocused)
            return;

        // E opens the interaction only when it is closed.
        if (distance <= interactionDistance &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactionOpen = true;

            dialogueUI.ShowDialogue(
                "COMPANION",
                "Hey, partner. What's on your mind?"
            );
        }
    }
}
