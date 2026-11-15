using UnityEngine;
using TMPro;

public class DialogueBoxResizer : MonoBehaviour
{
    public RectTransform panel;
    public TMP_Text dialogueText;
    public RectTransform speakerName;

    [Header("Panel Size")]
    public float minWidth = 1200f;
    public float maxWidth = 2200f;

    [Header("Padding")]
    public float horizontalPadding = 300f;
    public float verticalPadding = 100f;

    [Header("Layout")]
    public float speakerHeight = 150f;
    public float spacing = 40f;

    public void Resize()
    {
        dialogueText.enableWordWrapping = true;

        // Calculate how much space the text wants.
        Vector2 preferred = dialogueText.GetPreferredValues(
            dialogueText.text,
            maxWidth,
            0
        );

        float width = Mathf.Clamp(
            preferred.x + horizontalPadding,
            minWidth,
            maxWidth
        );

        // Calculate the text height using the final width.
        Vector2 finalTextSize = dialogueText.GetPreferredValues(
            dialogueText.text,
            width - horizontalPadding,
            0
        );

        float textHeight = finalTextSize.y;

        float height =
            speakerHeight +
            spacing +
            textHeight +
            verticalPadding;

        // Resize the actual panel.
        panel.sizeDelta = new Vector2(width, height);

        // Resize the text area.
        dialogueText.rectTransform.sizeDelta =
            new Vector2(
                width - horizontalPadding,
                textHeight
            );

        // Position speaker.
        speakerName.anchoredPosition =
            new Vector2(
                0,
                -speakerHeight / 2f
            );

        // Position dialogue text.
        dialogueText.rectTransform.anchoredPosition =
    new Vector2(
        0,
        -(speakerHeight + spacing + textHeight / 2f)
    );

        Debug.Log(
            "PANEL SIZE = " + panel.sizeDelta +
            " | TEXT SIZE = " + dialogueText.rectTransform.sizeDelta
        );
    }
}