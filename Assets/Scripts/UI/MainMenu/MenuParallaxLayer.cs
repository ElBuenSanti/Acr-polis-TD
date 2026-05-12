using UnityEngine;

public class MenuParallaxLayer : MonoBehaviour
{
    [SerializeField] private Vector2 moveAmount = new Vector2(15f, 8f);
    [SerializeField] private float speed = 3f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector2 normalizedMouse = new Vector2(
            Input.mousePosition.x / Screen.width - 0.5f,
            Input.mousePosition.y / Screen.height - 0.5f
        );

        Vector3 targetPosition = startPosition + new Vector3(
            normalizedMouse.x * moveAmount.x,
            normalizedMouse.y * moveAmount.y,
            0f
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.unscaledDeltaTime * speed
        );
    }
}