using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private CanvasGroup shopCanvasGroup;

    [Header("Construction Data")]
    [SerializeField] private ConstructionData[] shopConstructions;

    [Header("Cards")]
    [SerializeField] private RectTransform[] cardTransforms;
    [SerializeField] private Image[] cardHighlights;
    [SerializeField] private TextMeshProUGUI[] cardNames;
    [SerializeField] private TextMeshProUGUI[] cardCosts;

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI agapeText;
    [SerializeField] private TextMeshProUGUI iraText;
    [SerializeField] private TextMeshProUGUI merakiText;

    [Header("Settings")]
    [SerializeField] private int currentIndex;
    [SerializeField] private int maxIndex = 2;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Detail")]
    [SerializeField] private GameObject cardsArea;
    [SerializeField] private GameObject detailsArea;

    private void Awake()
    {
        if (shopCanvasGroup == null)
            shopCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        RefreshCards();
        RefreshResources();
        UpdateFocusVisual();
    }

    private void Update()
    {
        RefreshResources();
        UpdateFocusVisual();
        UpdateCardAnimation();
    }

    public void Open()
    {
        GameStateController.Instance.SetState(GameState.ShopOpen);
        RefreshCards();
    }

    public void Close()
    {
        GameStateController.Instance.SetState(GameState.MapIdle);
        RefreshCards();
    }

    public void Toggle()
    {
        if (GameStateController.Instance.currentState == GameState.ShopOpen)
            Close();
        else if (GameStateController.Instance.currentState == GameState.MapIdle)
            Open();
    }

    public void MoveLeft()
    {
        if (GameStateController.Instance.currentState != GameState.ShopOpen)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = maxIndex;

        RefreshCards();
    }

    public void MoveRight()
    {
        if (GameStateController.Instance.currentState != GameState.ShopOpen)
            return;

        currentIndex++;

        if (currentIndex > maxIndex)
            currentIndex = 0;

        RefreshCards();
    }

    public void ConfirmSelection()
    {
        if (GameStateController.Instance.currentState != GameState.ShopOpen)
            return;

        BuildingManager.Instance.SetConstructionIndex(currentIndex);
        GameStateController.Instance.SetState(GameState.PlacingTower);
        RefreshCards();
    }

    private void RefreshCards()
    {
        for (int i = 0; i < cardHighlights.Length; i++)
        {
            bool isSelected = i == currentIndex &&
                              GameStateController.Instance.currentState == GameState.ShopOpen;

            if (cardHighlights[i] != null)
                cardHighlights[i].enabled = isSelected;

            if (i < shopConstructions.Length && shopConstructions[i] != null)
            {
                if (cardNames[i] != null)
                    cardNames[i].text = shopConstructions[i].type.ToString();

                if (cardCosts[i] != null)
                    cardCosts[i].text = GetFirstCost(shopConstructions[i]).ToString("0");
            }
        }
    }

    private float GetFirstCost(ConstructionData data)
    {
        if (data.willToPay == null || data.willToPay.Count == 0)
            return 0;

        return data.willToPay[0].amount;
    }

    private void RefreshResources()
    {
        if (WillManager.Instance == null)
            return;

        if (agapeText != null)
            agapeText.text = WillManager.Instance.GetMoney(Will.Agape).ToString("0");

        if (iraText != null)
            iraText.text = WillManager.Instance.GetMoney(Will.Ira).ToString("0");

        if (merakiText != null)
            merakiText.text = WillManager.Instance.GetMoney(Will.Meraki).ToString("0");
    }

    private void UpdateFocusVisual()
    {
        if (shopCanvasGroup == null)
            return;

        shopCanvasGroup.alpha = GameStateController.Instance.currentState == GameState.ShopOpen ? 1f : 0.65f;
    }

    private void UpdateCardAnimation()
    {
        for (int i = 0; i < cardTransforms.Length; i++)
        {
            if (cardTransforms[i] == null)
                continue;

            float targetScale = i == currentIndex &&
                                GameStateController.Instance.currentState == GameState.ShopOpen
                ? selectedScale
                : normalScale;

            Vector3 target = Vector3.one * targetScale;

            cardTransforms[i].localScale = Vector3.Lerp(
                cardTransforms[i].localScale,
                target,
                Time.deltaTime * scaleSpeed
            );
        }
    }


    public void ShowCardsMode()
    {
        if (cardsArea != null) cardsArea.SetActive(true);
        if (detailsArea != null) detailsArea.SetActive(false);
    }

    public void ShowDetailsMode()
    {
        if (cardsArea != null) cardsArea.SetActive(false);
        if (detailsArea != null) detailsArea.SetActive(true);
    }
}