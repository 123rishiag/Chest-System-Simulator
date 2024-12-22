using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestController
{
    private float unlockTimeInMinutes;
    private float remainingTimeInSeconds;
    private bool isCurrencyUsedToUnlock;
    private CurrencyType chestUnlockCurrencyType;

    private GameManager gameManager;

    private GameObject chestObject;
    private Button chestButton;

    private Image chestImage;
    private TMP_Text chestMessageOneText;
    private TMP_Text chestMessageTwoText;
    private GameObject chestUnlockCurrencyPanel;
    private TMP_Text chestUnlockCurrencyText;
    private Image chestUnlockCurrencyImage;

    public ChestState ChestState { get; private set; }
    public ChestType ChestType { get; private set; }
    public ChestData ChestData { get; private set; }
    public ChestController(GameManager _gameManager, ChestData _chestData, Transform _parentTransform, GameObject _prefab)
    {
        gameManager = _gameManager;

        // Setting Chest Variables
        unlockTimeInMinutes = _chestData.unlockTimeInMinutes;
        remainingTimeInSeconds = unlockTimeInMinutes * 60;
        isCurrencyUsedToUnlock = false;
        chestUnlockCurrencyType = _chestData.chestUnlockCurrencyType;

        ChestState = ChestState.Locked;
        ChestType = _chestData.chestType;
        ChestData = _chestData;

        // Instantiating the prefab under the parent transform
        chestObject = Object.Instantiate(_prefab, _parentTransform);
        chestButton = chestObject.GetComponent<Button>();

        if (chestObject == null || chestButton == null)
        {
            Debug.LogError("Chest Prefab or Button not found!");
            return;
        }

        // Setting Chest UI
        chestImage = chestObject.GetComponent<Image>();
        if (chestImage == null)
        {
            Debug.LogError("Chest Image Field not found in prefab!!");
            return;
        }

        chestMessageOneText = chestObject.transform.Find("ChestMessageOnePanel").GetComponentInChildren<TMP_Text>();
        if (chestMessageOneText == null)
        {
            Debug.LogError("Chest Message 1 Text Field not found in prefab!!");
            return;
        }

        chestMessageTwoText = chestObject.transform.Find("ChestMessageTwoPanel").GetComponentInChildren<TMP_Text>();
        if (chestMessageTwoText == null)
        {
            Debug.LogError("Chest Message 2 Text Field not found in prefab!!");
            return;
        }

        chestUnlockCurrencyPanel = chestObject.transform.Find("ChestIcon/ChestUnlockCurrencyPanel").gameObject;
        if (chestUnlockCurrencyPanel == null)
        {
            Debug.LogError("Chest Unlock Currency Panel not found in prefab!!");
            return;
        }

        chestUnlockCurrencyText = chestObject.transform.Find("ChestIcon/ChestUnlockCurrencyPanel/ChestUnlockCurrencyContent/ChestUnlockCurrencyText").GetComponent<TMP_Text>();
        if (chestUnlockCurrencyText == null)
        {
            Debug.LogError("Chest Unlock Currency Text Field not found in prefab!!");
            return;
        }

        chestUnlockCurrencyImage = chestObject.transform.Find("ChestIcon/ChestUnlockCurrencyPanel/ChestUnlockCurrencyContent/ChestUnlockCurrencyImage").GetComponent<Image>();
        if (chestUnlockCurrencyImage == null)
        {
            Debug.LogError("Chest Unlock Currency Image Field not found in prefab!!");
            return;
        }

        UpdateUI();
    }

    public void Update()
    {
        PeformStateTransition();
        UpdateTimer();
        UpdateUI();
    }

    private void PeformStateTransition()
    {
        switch (ChestState)
        {
            case ChestState.Locked:
                chestUnlockCurrencyPanel.SetActive(true);
                chestButton.onClick.RemoveAllListeners();
                chestButton.onClick.AddListener(() =>
                {
                    gameManager.ConfigureButtons(
                        () => ProcessStartUnlocking(),
                        () => ProcessUnlockChestWithCurrency(),
                        "Start Timer",
                        $"Unlock With {ChestData.chestUnlockCurrencyType}s"
                    );
                });
                break;
            case ChestState.Unlocking:
                chestUnlockCurrencyPanel.SetActive(true);
                chestButton.onClick.RemoveAllListeners();
                ProcessUnlockChest();
                chestButton.onClick.AddListener(() =>
                {
                    gameManager.ConfigureButtons(
                        () => ProcessUnlockChestWithCurrency(),
                        () => { },
                        $"Unlock With {ChestData.chestUnlockCurrencyType}s",
                        "Cancel"
                    );
                });
                break;
            case ChestState.Unlocked:
                chestUnlockCurrencyPanel.SetActive(false);
                chestButton.onClick.RemoveAllListeners();
                chestButton.onClick.AddListener(() =>
                {
                    gameManager.ConfigureButtons(
                        () => ProcessCollectChest(),
                        () => ProcessUndoCurrencyPurchase(),
                        "Collect Chest",
                        GetUndoCurrencyPurchaseString()
                    );
                });
                break;
            case ChestState.Collected:
                chestUnlockCurrencyPanel.SetActive(false);
                chestButton.interactable = false;
                break;
            default:
                chestUnlockCurrencyPanel.SetActive(false);
                break;
        }
    }
    private void UpdateTimer()
    {
        if (ChestState == ChestState.Unlocking)
        {
            remainingTimeInSeconds -= Time.deltaTime;
        }
    }
    private void UpdateUI()
    {
        CurrencyData currencyData = gameManager.GetCurrencyData(chestUnlockCurrencyType);

        chestImage.color = GetImageColor();
        chestMessageOneText.text = FormatTime(remainingTimeInSeconds);
        chestMessageTwoText.text = $"{ChestType} : {ChestState}";
        chestUnlockCurrencyText.text = GetCurrencyRequiredToUnlock().ToString();
        chestUnlockCurrencyImage.sprite = currencyData.currencyImage;
        chestUnlockCurrencyImage.color = new Color(currencyData.imageColor.r, currencyData.imageColor.g, currencyData.imageColor.b, chestUnlockCurrencyImage.color.a);
    }

    private void ProcessStartUnlocking()
    {
        if (!gameManager.IsAnyChestUnlocking())
        {
            ChestState = ChestState.Unlocking;
            gameManager.ShowNotification("Timer Started!!");
        }
        else
        {
            gameManager.ShowNotification("Timer Already Running for another chest!!");
        }

    }
    private void ProcessUnlockChest()
    {
        if (remainingTimeInSeconds <= 0)
        {
            remainingTimeInSeconds = 0;
            ChestState = ChestState.Unlocked;
            gameManager.ShowNotification("Timer Completed. Chest Unlocked!!");
        }
    }
    private void ProcessUnlockChestWithCurrency()
    {
        int currencyRequired = GetCurrencyRequiredToUnlock();
        int currencyAvailable = gameManager.GetCurrency(chestUnlockCurrencyType);

        if (currencyRequired <= currencyAvailable && remainingTimeInSeconds > 0)
        {
            gameManager.DeductCurrency(chestUnlockCurrencyType, currencyRequired);
            ChestState = ChestState.Unlocked;
            isCurrencyUsedToUnlock = true;
            gameManager.ShowNotification($"Chest unlocked with {currencyRequired} {chestUnlockCurrencyType}s!!");
        }
        else
        {
            gameManager.ShowNotification($"Chest can't be unlocked. {currencyRequired} {chestUnlockCurrencyType}s required!!");
        }
    }
    private void ProcessCollectChest()
    {
        string rewardText = string.Empty;
        foreach (var reward in ChestData.rewards)
        {
            int rewardRandomValue = Random.Range(reward.minValue, reward.maxValue + 1);
            gameManager.AddCurrency(reward.currencyType, rewardRandomValue);
            rewardText += $" {rewardRandomValue} {reward.currencyType}s";
        }
        ChestState = ChestState.Collected;
        gameManager.ShowNotification($"Chest Collected!! You gained {rewardText}!!");
    }
    private void ProcessUndoCurrencyPurchase()
    {
        if (isCurrencyUsedToUnlock)
        {
            int currencyRequired = GetCurrencyRequiredToUnlock();
            gameManager.AddCurrency(chestUnlockCurrencyType, currencyRequired);
            ChestState = ChestState.Locked;
            isCurrencyUsedToUnlock = false;
            gameManager.ShowNotification($"Reverted {chestUnlockCurrencyType} chest unlock. You gained {currencyRequired} {chestUnlockCurrencyType}s!!");
        }
    }
    private string GetUndoCurrencyPurchaseString()
    {
        return isCurrencyUsedToUnlock ? $"Undo {ChestData.chestUnlockCurrencyType}s Purchase" : "Cancel";
    }
    private int GetCurrencyRequiredToUnlock()
    {
        // Convert minutes to seconds
        int chestUnlockSecondsSingleCurrency = ChestData.chestUnlockMinutesSingleCurrency * 60;

        // Calculate currency required with ceiling
        int currencyRequired = Mathf.CeilToInt((float)remainingTimeInSeconds / chestUnlockSecondsSingleCurrency);

        return currencyRequired;
    }
    private Color GetImageColor()
    {
        switch (ChestState)
        {
            case ChestState.Locked:
                return new Color(128, 128, 128, chestImage.color.a); // Grey
            case ChestState.Unlocking:
                return new Color(0, 0, 255, chestImage.color.a); // Blue
            case ChestState.Unlocked:
                return new Color(0, 255, 0, chestImage.color.a); // Green
            case ChestState.Collected:
                return new Color(255, 255, 0, chestImage.color.a); // Yellow
            default:
                return new Color(0, 0, 0, chestImage.color.a); // White
        }
    }

    private string FormatTime(float _timeInSeconds)
    {
        // Converting time from float to int
        int totalSeconds = Mathf.FloorToInt(_timeInSeconds);

        // Determining the time range
        switch (totalSeconds)
        {
            case < 60:
                // Less than 1 minute
                return $"{totalSeconds}sec";

            case < 3600:
                // Less than 1 hour
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                return $"{minutes}min {seconds}sec";

            case < 86400:
                // Less than 1 day
                int hours = totalSeconds / 3600;
                minutes = (totalSeconds % 3600) / 60;
                return $"{hours}H {minutes}min";

            default:
                // 1 day or more
                int days = totalSeconds / 86400;
                hours = (totalSeconds % 86400) / 3600;
                return $"{days}D {hours}H";
        }
    }

    public void Destroy()
    {
        Object.Destroy(chestObject);
    }
}