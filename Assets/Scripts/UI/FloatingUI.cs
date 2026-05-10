using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [SerializeField] private float amplitude = 8f;
    [SerializeField] private float frequency = 1f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector3 offset = Vector3.up *
            Mathf.Sin(Time.unscaledTime * frequency) *
            amplitude;

        transform.localPosition = startPosition + offset;
    }
}