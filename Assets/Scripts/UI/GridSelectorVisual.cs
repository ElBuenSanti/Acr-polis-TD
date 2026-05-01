using UnityEngine;

public class GridSelectorVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSelector gridSelector;
    [SerializeField] private Transform visualObject;

    [Header("Settings")]
    [SerializeField] private float yOffset = 0.15f;

    private void Awake()
    {
        if (gridSelector == null)
            gridSelector = FindAnyObjectByType<GridSelector>();
    }

    private void LateUpdate()
    {
        if (gridSelector == null || gridSelector.currentTile == null || visualObject == null)
            return;

        Vector3 targetPosition = gridSelector.currentTile.transform.position;
        targetPosition.y += yOffset;

        visualObject.position = targetPosition;
    }
}