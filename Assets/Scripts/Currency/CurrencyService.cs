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
        private UIService uiService;

        public CurrencyService(CurrencyConfig _currencyConfig)
        {
            // Setting Variables
            currencyConfig = _currencyConfig;
            currencyControllers = new List<CurrencyController>();

            // Validating References
            ValidateReferences();
        }

        public void Init(UIService _uiService)
        {
            // Setting Services
            uiService = _uiService;

            // Adding Currencies
            CreateCurrencies();
        }

        private void ValidateReferences()
        {
            if (currencyConfig == null)
            {
                Debug.LogWarning("Currency Scriptable Object reference is null!");
                return;
            }
        }

        private void CreateCurrencies()
        {
            // Initializing a CurrencyController for each currency in the config
            foreach (var currencyData in currencyConfig.currencies)
            {
                var currencyController = new CurrencyController(currencyData, uiService.currencyPanel, uiService.currencyPrefab);
                currencyControllers.Add(currencyController);
            }
        }

        // Getters
        public int GetCurrency(CurrencyType _currencyType)
        {
            var currencyController = GetCurrencyController(_currencyType);
            return currencyController.CurrencyValue;
        }
        public CurrencyData GetCurrencyData(CurrencyType _currencyType)
        {
            var currencyController = GetCurrencyController(_currencyType);
            return currencyController.CurrencyData;
        }
        private CurrencyController GetCurrencyController(CurrencyType _currencyType)
        {
            foreach (var currencyController in currencyControllers)
            {
                if (currencyController.CurrencyType == _currencyType)
                {
                    return currencyController;
                }
            }
            return null;
        }

        // Setters
        public void AddCurrency(CurrencyType _currencyType, int _value)
        {
            var currencyController = GetCurrencyController(_currencyType);
            currencyController.AddCurrency(_value);
        }
        public void DeductCurrency(CurrencyType _currencyType, int _value)
        {
            var currencyController = GetCurrencyController(_currencyType);
            currencyController.DeductCurrency(_value);
        }
    }
}