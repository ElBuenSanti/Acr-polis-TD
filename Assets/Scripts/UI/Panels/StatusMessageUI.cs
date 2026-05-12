using System.Collections;
using UnityEngine;
using TMPro;

public class StatusMessageUI : MonoBehaviour
{
    public static StatusMessageUI Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Animation")]
    [SerializeField] private float visibleTime = 2.2f;
    [SerializeField] private float fadeTime = 0.18f;
    [SerializeField] private float slideDistance = 18f;

    private Coroutine messageRoutine;
    private Vector2 startPosition;

    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (panelTransform == null)
            panelTransform = GetComponent<RectTransform>();

        startPosition = panelTransform.anchoredPosition;
    }

    private void Start()
    {
        HideInstant();
    }

    public void ShowMessage(string message)
    {
        if (statusText != null)
            statusText.text = message;

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        //canvasGroup.gameObject.SetActive(true); 

        float timer = 0f;
        Vector2 hiddenPosition = startPosition + Vector2.up * slideDistance;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;

            canvasGroup.alpha = t;
            panelTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, startPosition, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelTransform.anchoredPosition = startPosition;

        yield return new WaitForSeconds(visibleTime);

        timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;

            canvasGroup.alpha = 1f - t;
            panelTransform.anchoredPosition = Vector2.Lerp(startPosition, hiddenPosition, t);

            yield return null;
        }

        HideInstant();
    }

    private void HideInstant()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (panelTransform != null)
            panelTransform.anchoredPosition = startPosition;
    }
}