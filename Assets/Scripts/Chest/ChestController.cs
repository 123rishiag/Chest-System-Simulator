using ServiceLocator.Currency;
using ServiceLocator.Event;
using ServiceLocator.UI;
using UnityEngine;

namespace ServiceLocator.Chest
{
    public class ChestController
    {
        // Private Variables
        private ChestModel chestModel;
        private ChestView chestView;

        // Private Services
        private EventService eventService;
        private UIService uiService;
        private ChestService chestService;

        public ChestController(ChestData _chestData, Transform _parentTransform, ChestView _chestPrefab,
            EventService _eventService, UIService _uiService, ChestService _chestService)
        {
            // Setting Variables
            chestModel = new ChestModel(_chestData);
            chestView = Object.Instantiate(_chestPrefab, _parentTransform).GetComponent<ChestView>();

            // Setting Services
            eventService = _eventService;
            uiService = _uiService;
            chestService = _chestService;

            // Setting Elements
            chestModel.ChestUnlockCurrencyModel = eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(chestModel.ChestUnlockCurrencyType).GetCurrencyModel();
            chestView.SetViewProperties(this);
            chestView.UpdateUI();
        }

        public void Update()
        {
            PeformStateTransition();
            UpdateTimer();
            chestView.UpdateUI();
        }

        public void Destroy()
        {
            chestView.Destroy();
        }

        private void PeformStateTransition()
        {
            switch (chestModel.ChestState)
            {
                case ChestState.Locked:
                    chestView.chestUnlockCurrencyPanel.SetActive(true);
                    chestView.chestButton.onClick.RemoveAllListeners();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        uiService.GetUIController().ConfigureButtons(
                            () => ProcessStartUnlocking(),
                            () => ProcessUnlockChestWithCurrency(),
                            "Start Timer",
                            $"Unlock With {chestModel.ChestData.chestUnlockCurrencyType}s"
                        );
                    });
                    break;
                case ChestState.Unlock_Queue:
                    chestView.chestUnlockCurrencyPanel.SetActive(true);
                    chestView.chestButton.onClick.RemoveAllListeners();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        uiService.GetUIController().ConfigureButtons(
                            () => ProcessUnlockChestWithCurrency(),
                            () => ProcessStopUnlocking(),
                            $"Unlock With {chestModel.ChestData.chestUnlockCurrencyType}s",
                            "Remove from Queue"
                        );
                    });
                    break;
                case ChestState.Unlocking:
                    chestView.chestUnlockCurrencyPanel.SetActive(true);
                    chestView.chestButton.onClick.RemoveAllListeners();
                    ProcessUnlockChest();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        uiService.GetUIController().ConfigureButtons(
                            () => ProcessUnlockChestWithCurrency(),
                            () => ProcessStopUnlocking(),
                            $"Unlock With {chestModel.ChestData.chestUnlockCurrencyType}s",
                            "Stop Unlocking"
                        );
                    });
                    break;
                case ChestState.Unlocked:
                    chestView.chestUnlockCurrencyPanel.SetActive(false);
                    chestView.chestButton.onClick.RemoveAllListeners();
                    chestView.chestButton.onClick.AddListener(() =>
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
                    chestView.chestUnlockCurrencyPanel.SetActive(false);
                    chestView.chestButton.interactable = false;
                    break;
                default:
                    chestView.chestUnlockCurrencyPanel.SetActive(false);
                    break;
            }
        }
        private void UpdateTimer()
        {
            if (chestModel.ChestState == ChestState.Unlocking)
            {
                chestModel.RemainingTimeInSeconds -= Time.deltaTime;
            }
        }

        // Processing Methods
        private void ProcessStartUnlocking()
        {
            if (!chestService.IsAnyChestUnlocking())
            {
                chestModel.ChestState = ChestState.Unlocking;
                uiService.GetUIController().ShowNotification("Timer Started!!");
            }
            else
            {
                chestModel.ChestState = ChestState.Unlock_Queue;
                chestService.AddChestToQueue(this);
                uiService.GetUIController().ShowNotification("Timer Already Running for another chest. Adding to Queue!!");
            }

        }
        private void ProcessUnlockChest()
        {
            if (chestModel.RemainingTimeInSeconds <= 0)
            {
                chestModel.RemainingTimeInSeconds = 0;
                chestModel.ChestState = ChestState.Unlocked;
                uiService.GetUIController().ShowNotification("Timer Completed. Chest Unlocked!!");
            }
        }
        private void ProcessUnlockChestWithCurrency()
        {
            int currencyRequired = GetCurrencyRequiredToUnlock();
            int currencyAvailable = chestModel.ChestUnlockCurrencyModel.CurrencyValue;

            if (currencyRequired <= currencyAvailable && chestModel.RemainingTimeInSeconds > 0)
            {
                eventService.OnDeductCurrencyEvent.Invoke(chestModel.ChestUnlockCurrencyType, currencyRequired);
                chestModel.ChestState = ChestState.Unlocked;
                chestModel.IsCurrencyUsedToUnlock = true;
                uiService.GetUIController().ShowNotification($"Chest unlocked with {currencyRequired} {chestModel.ChestUnlockCurrencyType}s!!");
            }
            else
            {
                uiService.GetUIController().ShowNotification($"Chest can't be unlocked. {currencyRequired} {chestModel.ChestUnlockCurrencyType}s required!!");
            }
        }
        private void ProcessStopUnlocking()
        {
            chestModel.ChestState = ChestState.Locked;
            uiService.GetUIController().ShowNotification($"Chest stopped Unlocking!!");
        }
        private void ProcessCollectChest()
        {
            string rewardText = string.Empty;
            foreach (var reward in chestModel.ChestData.rewards)
            {
                int rewardRandomValue = Random.Range(reward.minValue, reward.maxValue + 1);
                eventService.OnAddCurrencyEvent.Invoke(reward.currencyType, rewardRandomValue);
                rewardText += $" {rewardRandomValue} {reward.currencyType}s";
            }
            chestModel.ChestState = ChestState.Collected;
            uiService.GetUIController().ShowNotification($"Chest Collected!! You gained {rewardText}!!");
        }
        private void ProcessUndoCurrencyPurchase()
        {
            if (chestModel.IsCurrencyUsedToUnlock)
            {
                int currencyRequired = GetCurrencyRequiredToUnlock();
                eventService.OnAddCurrencyEvent.Invoke(chestModel.ChestUnlockCurrencyType, currencyRequired);
                chestModel.ChestState = ChestState.Locked;
                chestModel.IsCurrencyUsedToUnlock = false;
                uiService.GetUIController().ShowNotification($"Reverted {chestModel.ChestUnlockCurrencyType} chest unlock. You gained {currencyRequired} {chestModel.ChestUnlockCurrencyType}s!!");
            }
        }

        // Getters
        private string GetUndoCurrencyPurchaseString()
        {
            return chestModel.IsCurrencyUsedToUnlock ? $"Undo {chestModel.ChestData.chestUnlockCurrencyType}s Purchase" : "Cancel";
        }
        public int GetCurrencyRequiredToUnlock()
        {
            // Convert minutes to seconds
            int chestUnlockSecondsSingleCurrency = chestModel.ChestData.chestUnlockMinutesSingleInCurrency * 60;

            // Calculate currency required with ceiling
            int currencyRequired = Mathf.CeilToInt((float)chestModel.RemainingTimeInSeconds / chestUnlockSecondsSingleCurrency);

            return currencyRequired;
        }
        public ChestModel GetChestModel()
        {
            return chestModel;
        }
    }
}