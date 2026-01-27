using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotator : MonoBehaviour
{
    public GameObject targetObject;
    public Vector2 rotationSpeed = new Vector2(0.1f, 0.2f);
    public bool reverse;

    private Vector2 lastMousePosition;
    private bool isDragging;

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
            isDragging = true;
        }
        else if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector2 currentPos = Mouse.current.position.ReadValue();
            Vector2 delta = currentPos - lastMousePosition;

            float x = reverse ? -delta.y : delta.y;
            float y = reverse ? delta.x : -delta.x;

            Vector3 newAngle = new Vector3(
                x * rotationSpeed.x,
                y * rotationSpeed.y,
                0
            );

            targetObject.transform.Rotate(newAngle, Space.World);
            lastMousePosition = currentPos;
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }
}
