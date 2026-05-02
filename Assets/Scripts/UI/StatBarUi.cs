using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private Image baseBar;
    [SerializeField] private Image changeBar;

    [Header("Colors")]
    [SerializeField] private Color baseColor = Color.gray;
    [SerializeField] private Color upgradeColor = Color.green;
    [SerializeField] private Color downgradeColor = Color.red;

    public void SetStat(string statName, float currentValue, float newValue, float maxValue)
    {
        if (statNameText != null)
            statNameText.text = statName;

        if (maxValue <= 0)
            maxValue = 1;

        float currentFill = Mathf.Clamp01(currentValue / maxValue);
        float newFill = Mathf.Clamp01(newValue / maxValue);

        SetBarRange(baseBar, 0f, currentFill, baseColor);

        if (Mathf.Approximately(currentFill, newFill))
        {
            SetBarRange(changeBar, 0f, 0f, upgradeColor);
            return;
        }

        if (newFill > currentFill)
        {
            SetBarRange(changeBar, currentFill - 0.01f, newFill, upgradeColor);
        }
        else
        {
            SetBarRange(changeBar, newFill, currentFill + 0.01f, downgradeColor);
        }
    }

    public void Clear()
    {
        if (statNameText != null)
            statNameText.text = "";

        SetBarRange(baseBar, 0f, 0f, baseColor);
        SetBarRange(changeBar, 0f, 0f, upgradeColor);
    }

    private void SetBarRange(Image image, float minFill, float maxFill, Color color)
    {
        if (image == null)
            return;

        RectTransform rect = image.rectTransform;

        rect.anchorMin = new Vector2(0f, minFill);
        rect.anchorMax = new Vector2(1f, maxFill);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        image.color = color;
    }
}