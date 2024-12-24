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
        private ChestService chestService;

        public ChestController(ChestData _chestData, Transform _parentTransform, ChestView _chestPrefab,
            EventService _eventService, ChestService _chestService)
        {
            // Setting Variables
            chestModel = new ChestModel(_chestData);
            chestView = Object.Instantiate(_chestPrefab, _parentTransform).GetComponent<ChestView>();

            // Setting Services
            eventService = _eventService;
            chestService = _chestService;

            // Setting Elements
            chestModel.ChestUnlockCurrencyModel = eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(chestModel.ChestUnlockCurrencyType).GetCurrencyModel();
            chestView.SetViewProperties(this);
            chestView.UpdateUI();
        }

        public void Reset(ChestData _chestData, int _viewIndex)
        {
            chestModel.ResetModel(_chestData);
            chestView.ResetView(_viewIndex);
        }

        public void Update()
        {
            PeformStateTransition();
            UpdateTimer();
            chestView.UpdateUI();
        }

        private void PeformStateTransition()
        {
            switch (chestModel.ChestState)
            {
                case ChestState.Locked:
                    chestView.chestUnlockCurrencyPanel.SetActive(true);
                    chestView.chestButton.interactable = true;
                    chestView.chestButton.onClick.RemoveAllListeners();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        eventService.OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
                            () => ProcessStartUnlocking(),
                            () => ProcessUnlockChestWithCurrency(),
                            "Start Timer",
                            $"Unlock With {chestModel.ChestData.chestUnlockCurrencyType}s"
                        );
                    });
                    break;
                case ChestState.Unlock_Queue:
                    chestView.chestUnlockCurrencyPanel.SetActive(true);
                    chestView.chestButton.interactable = true;
                    chestView.chestButton.onClick.RemoveAllListeners();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        eventService.OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
                            () => ProcessUnlockChestWithCurrency(),
                            () => { },
                            $"Unlock With {chestModel.ChestData.chestUnlockCurrencyType}s",
                            "Cancel"
                        );
                    });
                    break;
                case ChestState.Unlocking:
                    chestView.chestUnlockCurrencyPanel.SetActive(true);
                    chestView.chestButton.interactable = true;
                    chestView.chestButton.onClick.RemoveAllListeners();
                    ProcessUnlockChest();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        eventService.OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
                            () => ProcessUnlockChestWithCurrency(),
                            () => ProcessStopUnlocking(),
                            $"Unlock With {chestModel.ChestData.chestUnlockCurrencyType}s",
                            "Stop Unlocking"
                        );
                    });
                    break;
                case ChestState.Unlocked:
                    chestView.chestUnlockCurrencyPanel.SetActive(false);
                    chestView.chestButton.interactable = true;
                    chestView.chestButton.onClick.RemoveAllListeners();
                    chestView.chestButton.onClick.AddListener(() =>
                    {
                        eventService.OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
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
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification("Timer Started!!");
            }
            else
            {
                chestModel.ChestState = ChestState.Unlock_Queue;
                chestService.AddChestToQueue(this);
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification("Timer Already Running for another chest. Adding to Queue!!");
            }

        }
        private void ProcessUnlockChest()
        {
            if (chestModel.RemainingTimeInSeconds <= 0)
            {
                chestModel.RemainingTimeInSeconds = 0;
                chestModel.ChestState = ChestState.Unlocked;
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification("Timer Completed. Chest Unlocked!!");
            }
        }
        private void ProcessUnlockChestWithCurrency()
        {
            int currencyRequired = GetCurrencyRequiredToUnlock();
            int currencyAvailable = chestModel.ChestUnlockCurrencyModel.CurrencyValue;

            if (currencyRequired <= currencyAvailable && chestModel.RemainingTimeInSeconds > 0)
            {
                eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(chestModel.ChestUnlockCurrencyType).DeductCurrency(currencyRequired);
                chestModel.ChestState = ChestState.Unlocked;
                chestModel.IsCurrencyUsedToUnlock = true;
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Chest unlocked with {currencyRequired} {chestModel.ChestUnlockCurrencyType}s!!");
            }
            else
            {
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Chest can't be unlocked. {currencyRequired} {chestModel.ChestUnlockCurrencyType}s required!!");
            }
        }
        private void ProcessStopUnlocking()
        {
            chestModel.ChestState = ChestState.Locked;
            eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Chest stopped Unlocking!!");
        }
        private void ProcessCollectChest()
        {
            string rewardText = string.Empty;
            foreach (var reward in chestModel.ChestData.rewards)
            {
                int rewardRandomValue = Random.Range(reward.minValue, reward.maxValue + 1);
                eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(reward.currencyType).AddCurrency(rewardRandomValue);
                rewardText += $" {rewardRandomValue} {reward.currencyType}s";
            }
            chestModel.ChestState = ChestState.Collected;
            eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Chest Collected!! You gained {rewardText}!!");
        }
        private void ProcessUndoCurrencyPurchase()
        {
            if (chestModel.IsCurrencyUsedToUnlock)
            {
                int currencyRequired = GetCurrencyRequiredToUnlock();
                eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(chestModel.ChestUnlockCurrencyType).AddCurrency(currencyRequired);
                chestModel.ChestState = ChestState.Locked;
                chestModel.IsCurrencyUsedToUnlock = false;
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Reverted {chestModel.ChestUnlockCurrencyType} chest unlock. You gained {currencyRequired} {chestModel.ChestUnlockCurrencyType}s!!");
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