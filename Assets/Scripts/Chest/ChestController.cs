using ServiceLocator.Currency;
using ServiceLocator.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ServiceLocator.Chest
{
    public class ChestController
    {
        // Private Variables
        private float unlockTimeInMinutes;
        private float remainingTimeInSeconds;
        private CurrencyType chestUnlockCurrencyType;

        private GameObject chestObject;
        private Button chestButton;

        private Image chestImage;
        private TMP_Text chestMessageOneText;
        private TMP_Text chestMessageTwoText;
        private GameObject chestUnlockCurrencyPanel;
        private TMP_Text chestUnlockCurrencyText;
        private Image chestUnlockCurrencyImage;

        public bool IsCurrencyUsedToUnlock { get; private set; }
        public ChestState ChestState { get; set; }
        public ChestType ChestType { get; private set; }
        public ChestData ChestData { get; private set; }

        // Private Services
        private UIService uiService;
        private CurrencyService currencyService;
        private ChestService chestService;

        public ChestController(ChestData _chestData, Transform _parentTransform, GameObject _prefab,
            UIService _uiService, CurrencyService _currencyService, ChestService _chestService)
        {
            // Setting Variables
            unlockTimeInMinutes = _chestData.unlockTimeInMinutes;
            remainingTimeInSeconds = unlockTimeInMinutes * 60;
            IsCurrencyUsedToUnlock = false;
            chestUnlockCurrencyType = _chestData.chestUnlockCurrencyType;

            ChestState = ChestState.Locked;
            ChestType = _chestData.chestType;
            ChestData = _chestData;

            // Setting Services
            uiService = _uiService;
            currencyService = _currencyService;
            chestService = _chestService;

            // Instantiating the prefab under the parent transform
            chestObject = Object.Instantiate(_prefab, _parentTransform);
            chestButton = chestObject.GetComponent<Button>();

            if (chestObject == null || chestButton == null)
            {
                Debug.LogError("Chest Prefab or Button not found!!");
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
                        uiService.GetUIController().ConfigureButtons(
                            () => ProcessStartUnlocking(),
                            () => ProcessUnlockChestWithCurrency(),
                            "Start Timer",
                            $"Unlock With {ChestData.chestUnlockCurrencyType}s"
                        );
                    });
                    break;
                case ChestState.Unlock_Queue:
                    chestUnlockCurrencyPanel.SetActive(true);
                    chestButton.onClick.RemoveAllListeners();
                    chestButton.onClick.AddListener(() =>
                    {
                        uiService.GetUIController().ConfigureButtons(
                            () => ProcessUnlockChestWithCurrency(),
                            () => ProcessStopUnlocking(),
                            $"Unlock With {ChestData.chestUnlockCurrencyType}s",
                            "Remove from Queue"
                        );
                    });
                    break;
                case ChestState.Unlocking:
                    chestUnlockCurrencyPanel.SetActive(true);
                    chestButton.onClick.RemoveAllListeners();
                    ProcessUnlockChest();
                    chestButton.onClick.AddListener(() =>
                    {
                        uiService.GetUIController().ConfigureButtons(
                            () => ProcessUnlockChestWithCurrency(),
                            () => ProcessStopUnlocking(),
                            $"Unlock With {ChestData.chestUnlockCurrencyType}s",
                            "Stop Unlocking"
                        );
                    });
                    break;
                case ChestState.Unlocked:
                    chestUnlockCurrencyPanel.SetActive(false);
                    chestButton.onClick.RemoveAllListeners();
                    chestButton.onClick.AddListener(() =>
                    {
                        uiService.GetUIController().ConfigureButtons(
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
            CurrencyData currencyData = currencyService.GetCurrencyController(chestUnlockCurrencyType).GetCurrencyModel().CurrencyData;

            chestImage.color = GetImageColor();
            chestMessageOneText.text = FormatTime(remainingTimeInSeconds);
            chestMessageTwoText.text = $"{ChestType} : {ChestState}";
            chestUnlockCurrencyText.text = GetCurrencyRequiredToUnlock().ToString();
            chestUnlockCurrencyImage.sprite = currencyData.currencyImage;
            chestUnlockCurrencyImage.color = new Color(currencyData.imageColor.r, currencyData.imageColor.g, currencyData.imageColor.b, chestUnlockCurrencyImage.color.a);
        }

        private void ProcessStartUnlocking()
        {
            if (!chestService.IsAnyChestUnlocking())
            {
                ChestState = ChestState.Unlocking;
                uiService.GetUIController().ShowNotification("Timer Started!!");
            }
            else
            {
                ChestState = ChestState.Unlock_Queue;
                chestService.AddChestToQueue(this);
                uiService.GetUIController().ShowNotification("Timer Already Running for another chest. Adding to Queue!!");
            }

        }
        private void ProcessUnlockChest()
        {
            if (remainingTimeInSeconds <= 0)
            {
                remainingTimeInSeconds = 0;
                ChestState = ChestState.Unlocked;
                uiService.GetUIController().ShowNotification("Timer Completed. Chest Unlocked!!");
            }
        }
        private void ProcessUnlockChestWithCurrency()
        {
            int currencyRequired = GetCurrencyRequiredToUnlock();
            int currencyAvailable = currencyService.GetCurrencyController(chestUnlockCurrencyType).GetCurrencyModel().CurrencyValue;

            if (currencyRequired <= currencyAvailable && remainingTimeInSeconds > 0)
            {
                currencyService.DeductCurrency(chestUnlockCurrencyType, currencyRequired);
                ChestState = ChestState.Unlocked;
                IsCurrencyUsedToUnlock = true;
                uiService.GetUIController().ShowNotification($"Chest unlocked with {currencyRequired} {chestUnlockCurrencyType}s!!");
            }
            else
            {
                uiService.GetUIController().ShowNotification($"Chest can't be unlocked. {currencyRequired} {chestUnlockCurrencyType}s required!!");
            }
        }
        private void ProcessStopUnlocking()
        {
            ChestState = ChestState.Locked;
            uiService.GetUIController().ShowNotification($"Chest stopped Unlocking!!");
        }
        private void ProcessCollectChest()
        {
            string rewardText = string.Empty;
            foreach (var reward in ChestData.rewards)
            {
                int rewardRandomValue = Random.Range(reward.minValue, reward.maxValue + 1);
                currencyService.AddCurrency(reward.currencyType, rewardRandomValue);
                rewardText += $" {rewardRandomValue} {reward.currencyType}s";
            }
            ChestState = ChestState.Collected;
            uiService.GetUIController().ShowNotification($"Chest Collected!! You gained {rewardText}!!");
        }
        private void ProcessUndoCurrencyPurchase()
        {
            if (IsCurrencyUsedToUnlock)
            {
                int currencyRequired = GetCurrencyRequiredToUnlock();
                currencyService.AddCurrency(chestUnlockCurrencyType, currencyRequired);
                ChestState = ChestState.Locked;
                IsCurrencyUsedToUnlock = false;
                uiService.GetUIController().ShowNotification($"Reverted {chestUnlockCurrencyType} chest unlock. You gained {currencyRequired} {chestUnlockCurrencyType}s!!");
            }
        }
        private string GetUndoCurrencyPurchaseString()
        {
            return IsCurrencyUsedToUnlock ? $"Undo {ChestData.chestUnlockCurrencyType}s Purchase" : "Cancel";
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
                case ChestState.Unlock_Queue:
                    return new Color(128, 0, 128, chestImage.color.a); // Purple
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
            int days, hours, minutes, seconds = 0;

            // Determining the time range
            switch (totalSeconds)
            {
                case < 60:
                    // Less than 1 minute
                    return $"{totalSeconds}sec";

                case < 3600:
                    // Less than 1 hour
                    minutes = totalSeconds / 60;
                    seconds = totalSeconds % 60;
                    return $"{minutes}min {seconds}sec";

                case < 86400:
                    // Less than 1 day
                    hours = totalSeconds / 3600;
                    minutes = (totalSeconds % 3600) / 60;
                    seconds = totalSeconds % 60;
                    return $"{hours}H {minutes}min {seconds}sec";

                default:
                    // 1 day or more
                    days = totalSeconds / 86400;
                    hours = (totalSeconds % 86400) / 3600;
                    minutes = (totalSeconds % 3600) / 60;
                    return $"{days}D {hours}H {minutes}min";
            }
        }

        public void Destroy()
        {
            Object.Destroy(chestObject);
        }
    }
}