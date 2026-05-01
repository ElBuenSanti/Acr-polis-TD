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
    [SerializeField] private Color upgradeColor = Color.green;
    [SerializeField] private Color downgradeColor = Color.red;
    [SerializeField] private Color sameColor = Color.yellow;

    public void SetStat(string statName, float currentValue, float newValue, float maxValue)
    {
        if (statNameText != null)
            statNameText.text = statName;

        if (maxValue <= 0)
            maxValue = 1;

        float currentFill = Mathf.Clamp01(currentValue / maxValue);
        float newFill = Mathf.Clamp01(newValue / maxValue);

        if (baseBar != null)
            baseBar.fillAmount = currentFill;

        if (changeBar != null)
        {
            changeBar.fillAmount = newFill;

            if (newValue > currentValue)
                changeBar.color = upgradeColor;
            else if (newValue < currentValue)
                changeBar.color = downgradeColor;
            else
                changeBar.color = sameColor;
        }
    }

    public void Clear()
    {
        if (statNameText != null)
            statNameText.text = "";

        if (baseBar != null)
            baseBar.fillAmount = 0;

        if (changeBar != null)
            changeBar.fillAmount = 0;
    }
}