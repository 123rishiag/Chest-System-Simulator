using UnityEngine;

namespace ServiceLocator.Currency
{
    public class CurrencyController
    {
        // Private Variables
        private CurrencyModel currencyModel;
        private CurrencyView currencyView;

        public CurrencyController(CurrencyData _currencyData, Transform _parentTransform, CurrencyView _currencyPrefab)
        {
            // Setting Variables
            currencyModel = new CurrencyModel(_currencyData);
            currencyView = Object.Instantiate(_currencyPrefab, _parentTransform).GetComponent<CurrencyView>();

            // Setting Elements
            currencyView.SetViewProperties(this);
            currencyView.UpdateUI();
        }

        // Getters
        public CurrencyModel GetCurrencyModel()
        {
            return currencyModel;
        }

        // Setters
        public void AddCurrency(int _value)
        {
            currencyModel.AddCurrency(_value);
            currencyView.UpdateUI();
        }
        public void DeductCurrency(int _value)
        {
            currencyModel.DeductCurrency(_value);
            currencyView.UpdateUI();
        }
    }
}