using UnityEngine;

public class RadialMenuUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas radialCanvas;

    [Header("Selection")]
    [SerializeField] private RadialOption selectedOption = RadialOption.None;

    private ConstructionController selectedConstruction;

    void Start()
    {
        Close();
    }

    public void Open(ConstructionController construction)
    {
        selectedConstruction = construction;
        selectedOption = RadialOption.None;
        radialCanvas.enabled = true;
    }

    public void Close()
    {
        selectedConstruction = null;
        selectedOption = RadialOption.None;

        if (radialCanvas != null)
            radialCanvas.enabled = false;
    }

    public void ReadStick(Vector2 input)
    {
        if (!radialCanvas.enabled)
            return;

        if (input.magnitude < 0.5f)
            return;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            selectedOption = input.x > 0 ? RadialOption.Ares : RadialOption.Hephaestus;
        }
        else
        {
            selectedOption = input.y > 0 ? RadialOption.Aphrodite : RadialOption.Sell;
        }

        Debug.Log("Radial option: " + selectedOption);
    }

    public void Confirm()
    {
        if (selectedConstruction == null)
            return;

        if (selectedOption == RadialOption.None)
            return;

        switch (selectedOption)
        {
            case RadialOption.Hephaestus:
                selectedConstruction.SetSelectedGod(GodType.Hephaestus);
                selectedConstruction.Upgrade();
                break;

            case RadialOption.Aphrodite:
                selectedConstruction.SetSelectedGod(GodType.Aphrodite);
                selectedConstruction.Upgrade();
                break;

            case RadialOption.Ares:
                selectedConstruction.SetSelectedGod(GodType.Ares);
                selectedConstruction.Upgrade();
                break;

            case RadialOption.Sell:
                Debug.Log("Sell is not implemented yet.");
                break;
        }

        Close();
        GameStateController.Instance.SetState(GameState.MapIdle);
    }
}