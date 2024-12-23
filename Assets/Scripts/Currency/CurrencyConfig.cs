using UnityEngine;

namespace ServiceLocator.Currency
{
    [CreateAssetMenu(fileName = "CurrencyConfig", menuName = "ScriptableObjects/CurrencyConfig")]
    public class CurrencyConfig : ScriptableObject
    {
        public CurrencyView currencyPrefab;
        public CurrencyData[] currencies;
    }

    [System.Serializable]
    public class CurrencyData
    {
        public CurrencyType currencyType;
        public int initialValue;
        public Sprite currencyImage;
        public Color imageColor;
    }
}