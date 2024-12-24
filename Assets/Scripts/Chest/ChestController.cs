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
        private ChestStateMachine chestStateMachine;

        // Private Services
        private EventService eventService;
        private ChestService chestService;

        public ChestController(ChestData _chestData, Transform _parentTransform, ChestView _chestPrefab,
            EventService _eventService, ChestService _chestService)
        {
            // Setting Variables
            chestModel = new ChestModel(_chestData);
            chestView = Object.Instantiate(_chestPrefab, _parentTransform).GetComponent<ChestView>();
            CreateStateMachine();
            chestStateMachine.ChangeState(ChestState.Locked);

            // Setting Services
            eventService = _eventService;
            chestService = _chestService;

            // Setting Elements
            chestModel.ChestUnlockCurrencyModel = eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(chestModel.ChestUnlockCurrencyType).GetCurrencyModel();
            chestView.SetViewProperties(this);
            chestView.UpdateUI();
        }

        private void CreateStateMachine() => chestStateMachine = new ChestStateMachine(this);

        public void Reset(ChestData _chestData, int _viewIndex)
        {
            chestModel.ResetModel(_chestData);
            chestStateMachine.ChangeState(ChestState.Locked);
            chestView.ResetView(_viewIndex);
        }

        public void Update()
        {
            chestStateMachine.Update();
            chestView.UpdateUI();
        }

        // Processing Methods
        public void UnlockWithCurrency()
        {
            int currencyRequired = GetCurrencyRequiredToUnlock();
            int currencyAvailable = chestModel.ChestUnlockCurrencyModel.CurrencyValue;

            // If Currency in stock is enough to purchase the price and timer for chest hasn't runout
            // Then, unlocking the chest, reducing the currency and setting Chest State to "Unlocked"
            if (currencyRequired <= currencyAvailable && chestModel.RemainingTimeInSeconds > 0)
            {
                chestStateMachine.ChangeState(ChestState.Unlocked);
                chestModel.IsCurrencyUsedToUnlock = true;

                // Deducting Currency
                eventService.OnGetCurrencyControllerEvent.Invoke<CurrencyController>(
                    chestModel.ChestUnlockCurrencyType).DeductCurrency(currencyRequired);

                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                    $"Chest unlocked with {currencyRequired} {chestModel.ChestUnlockCurrencyType}s!!");
            }
            else
            {
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                    $"Chest can't be unlocked. {currencyRequired} {chestModel.ChestUnlockCurrencyType}s required!!");
            }
        }

        // Getters
        public string GetUndoCurrencyPurchaseString()
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
        public ChestModel GetChestModel() => chestModel;
        public ChestView GetChestView() => chestView;
        public ChestStateMachine GetChestStateMachine() => chestStateMachine;
        public EventService GetEventService() => eventService;
        public ChestService GetChestService() => chestService;
    }
}