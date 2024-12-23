namespace ServiceLocator.Currency
{
    public class CurrencyModel
    {
        public CurrencyModel(CurrencyData _currencyData)
        {
            // Setting Variables
            CurrencyData = _currencyData;
            CurrencyType = _currencyData.currencyType;
            CurrencyValue = _currencyData.initialValue;
        }

        // Getters
        public CurrencyData CurrencyData { get; private set; }
        public CurrencyType CurrencyType { get; private set; }
        public int CurrencyValue { get; private set; }

        // Setters
        public void AddCurrency(int _value)
        {
            CurrencyValue += _value;
        }
        public void DeductCurrency(int _value)
        {
            CurrencyValue -= _value;
            if (CurrencyValue < 0)
            {
                CurrencyValue = 0;
            }
        }
    }
}