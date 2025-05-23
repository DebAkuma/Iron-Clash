using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Health Bars")]
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private Slider opponentHealthBar;

    [Header("Character Icons")]
    [SerializeField] private Image playerCharIcon;
    [SerializeField] private Image opponentCharIcon;

    void Awake()
    {
        InitializeHealthBars(100);
    }

    // Initialize both health bars
    public void InitializeHealthBars(int maxHealth)
    {
        playerHealthBar.maxValue = maxHealth;
        playerHealthBar.value = maxHealth;

        opponentHealthBar.maxValue = maxHealth;
        opponentHealthBar.value = maxHealth;
    }

    // Set character icons

    public void SetCharacterIcons(string playerName, string opponentName)
    {
        CharacterData playerData = GameManager.Instance.GetCharacter(playerName);
        CharacterData opponentData = GameManager.Instance.GetCharacter(opponentName);

        if (playerData != null && playerData.icon != null)
            playerCharIcon.sprite = playerData.icon;
        else
            Debug.LogWarning($"Icon not found for player: {playerName}");

        if (opponentData != null && opponentData.icon != null)
            opponentCharIcon.sprite = opponentData.icon;
        else
            Debug.LogWarning($"Icon not found for opponent: {opponentName}");
    }

    // Update player health instantly
    public void UpdatePlayerHealth(int currentHealth)
    {
        playerHealthBar.value = currentHealth;
    }

    // Update opponent health instantly
    public void UpdateOpponentHealth(int currentHealth)
    {
        opponentHealthBar.value = currentHealth;
    }

    // Optional: Smooth animation for player health
    public void UpdatePlayerHealthSmooth(int currentHealth)
    {
        StopCoroutine(nameof(SmoothPlayerHealth));
        StartCoroutine(SmoothPlayerHealth(currentHealth));
    }

    private IEnumerator SmoothPlayerHealth(int target)
    {
        while (Mathf.Abs(playerHealthBar.value - target) > 0.1f)
        {
            playerHealthBar.value = Mathf.Lerp(playerHealthBar.value, target, Time.deltaTime * 10);
            yield return null;
        }
        playerHealthBar.value = target;
    }

    // Optional: Smooth animation for opponent health
    public void UpdateOpponentHealthSmooth(int currentHealth)
    {
        StopCoroutine(nameof(SmoothOpponentHealth));
        StartCoroutine(SmoothOpponentHealth(currentHealth));
    }

    private IEnumerator SmoothOpponentHealth(int target)
    {
        while (Mathf.Abs(opponentHealthBar.value - target) > 0.1f)
        {
            opponentHealthBar.value = Mathf.Lerp(opponentHealthBar.value, target, Time.deltaTime * 10);
            yield return null;
        }
        opponentHealthBar.value = target;
    }
}
