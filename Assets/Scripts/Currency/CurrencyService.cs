using ServiceLocator.Event;
using ServiceLocator.UI;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceLocator.Currency
{
    public class CurrencyService
    {
        // Private Variables
        private CurrencyConfig currencyConfig;
        private List<CurrencyController> currencyControllers;

        // Private Services
        private EventService eventService;

        public CurrencyService(CurrencyConfig _currencyConfig, EventService _eventService)
        {
            // Setting Variables
            currencyConfig = _currencyConfig;
            currencyControllers = new List<CurrencyController>();

            // Setting Services
            eventService = _eventService;

            // Validating References
            ValidateReferences();

            // Adding Currencies
            CreateCurrencies();

            // Adding Listeners
            eventService.OnGetCurrencyControllerEvent.AddListener(GetCurrencyController);
        }

        ~CurrencyService()
        {
            // Removing Listeners
            eventService.OnGetCurrencyControllerEvent.RemoveListener(GetCurrencyController);
        }

        private void ValidateReferences()
        {
            if (currencyConfig == null)
            {
                Debug.LogError("Currency Scriptable Object reference is null!!");
                return;
            }

            if (currencyConfig.currencyPrefab == null)
            {
                Debug.LogError("Currency Prefab reference is null!!");
                return;
            }
        }

        private void CreateCurrencies()
        {
            // Initializing a CurrencyController for each currency in the config
            foreach (var currencyData in currencyConfig.currencies)
            {
                var currencyController = new CurrencyController(currencyData,
                    eventService.OnGetUIControllerEvent.Invoke<UIController>().GetUIView().currencyPanel,
                    currencyConfig.currencyPrefab);
                currencyControllers.Add(currencyController);
            }
        }

        // Getters
        private CurrencyController GetCurrencyController(CurrencyType _currencyType)
        {
            return currencyControllers.Find(controller => controller.GetCurrencyModel().CurrencyType == _currencyType);
        }
    }
}