using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlessingChoiceUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button[] blessingButtons;
    [SerializeField] private BlessingCardHighlightUI[] cardHighlights;

    [Header("Sound")]
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 8f;

    [Header("Navigation")]
    [SerializeField] private float inputDeadZone = 0.6f;
    [SerializeField] private float confirmDelay = 0.25f;

    private bool isOpen;
    private bool stickInUse;
    private int currentIndex;
    private float openTime;
    private ConstructionController templeController;

    private void Start()
    {
        HideInstant();
    }

    private void Update()
    {
        if (canvasGroup == null)
            return;

        float targetAlpha = isOpen ? 1f : 0f;

        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * fadeSpeed
        );
    }

    public void Open(ConstructionController temple)
    {
        templeController = temple;
        isOpen = true;
        currentIndex = 0;
        stickInUse = false;
        openTime = Time.unscaledTime;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayBlessingOpen();

        GameStateController.Instance.SetState(GameState.BlessingSelection);

        SelectCurrentButton();

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("Elige una condición de victoria");

        Debug.Log("Blessing panel opened. State: " + GameStateController.Instance.currentState);
    }

    public void Close()
    {
        isOpen = false;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayClosePanel();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    public void Move(Vector2 input)
    {
        if (!isOpen)
            return;

        if (Mathf.Abs(input.x) < inputDeadZone)
        {
            stickInUse = false;
            return;
        }

        if (stickInUse)
            return;

        stickInUse = true;

        if (input.x > 0)
            currentIndex++;
        else
            currentIndex--;

        currentIndex = Mathf.Clamp(currentIndex, 0, blessingButtons.Length - 1);

        SelectCurrentButton();
    }

    public void Confirm()
    {
        if (!isOpen)
            return;

        if (Time.unscaledTime < openTime + confirmDelay)
            return;

        if (currentIndex < 0 || currentIndex >= blessingButtons.Length)
            return;

        if (blessingButtons[currentIndex] == null)
            return;

        blessingButtons[currentIndex].onClick.Invoke();
    }

    private void SelectCurrentButton()
    {
        if (blessingButtons == null || blessingButtons.Length == 0)
            return;

        currentIndex = Mathf.Clamp(currentIndex, 0, blessingButtons.Length - 1);

        Button selectedButton = blessingButtons[currentIndex];

        if (selectedButton == null)
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);

        selectedButton.Select();

        if (cardHighlights != null)
        {
            for (int i = 0; i < cardHighlights.Length; i++)
            {
                if (cardHighlights[i] != null)
                    cardHighlights[i].SetSelected(i == currentIndex);
            }
        }

        Debug.Log("Blessing selected index: " + currentIndex);
    }

    private void HideInstant()
    {
        isOpen = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ChooseAphrodite()
    {
        ChooseGod(GodType.Aphrodite);
    }

    public void ChooseAres()
    {
        ChooseGod(GodType.Ares);
    }

    public void ChooseHephaestus()
    {
        ChooseGod(GodType.Hephaestus);
    }

    private void ChooseGod(GodType god)
    {
        if (!isOpen)
            return;

        if (Time.unscaledTime < openTime + confirmDelay)
            return;

        if (templeController == null)
            return;

        Debug.Log("Choosing blessing: " + god);

        templeController.SetSelectedGod(god);
        templeController.MarkBlessingChosen();
        templeController.Upgrade();

        BaseConstruction baseConstruction = templeController.GetComponent<BaseConstruction>();

        if (baseConstruction != null && baseConstruction.Data != null)
        {
            Debug.Log("Temple after upgrade: "
                + baseConstruction.Data.type + " / "
                + baseConstruction.Data.god + " / Lv "
                + baseConstruction.Data.level);
        }

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayBlessing(god);

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("Condición elegida: " + god);

        Close();
    }
}