using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Color sameColor = Color.clear;

    public void SetStat(string statName, float currentValue, float nextValue, float maxValue)
    {
        if (statNameText != null)
            statNameText.text = statName;

        float currentFill = maxValue <= 0 ? 0 : Mathf.Clamp01(currentValue / maxValue);
        float nextFill = maxValue <= 0 ? 0 : Mathf.Clamp01(nextValue / maxValue);

        if (baseBar != null)
        {
            baseBar.color = baseColor;
            baseBar.fillAmount = currentFill;
        }

        if (changeBar == null)
            return;

        if (Mathf.Approximately(currentFill, nextFill))
        {
            changeBar.color = sameColor;
            changeBar.fillAmount = 0f;
            return;
        }

        if (nextFill > currentFill)
        {
            changeBar.color = upgradeColor;
            changeBar.fillAmount = nextFill;
        }
        else
        {
            changeBar.color = downgradeColor;
            changeBar.fillAmount = currentFill;
        }
    }

    public void Clear()
    {
        if (statNameText != null)
            statNameText.text = "";

        if (baseBar != null)
            baseBar.fillAmount = 0f;

        if (changeBar != null)
            changeBar.fillAmount = 0f;
    }
}