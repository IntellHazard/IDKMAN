using UnityEngine;

public class DialogueBillboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        transform.LookAt(mainCamera.transform);
        transform.Rotate(0f, 180f, 0f);

        // Keep the UI upright.
        transform.rotation = Quaternion.Euler(
            0f,
            transform.rotation.eulerAngles.y,
            0f
        );
    }
}