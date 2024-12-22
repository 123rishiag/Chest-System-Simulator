using ServiceLocator.Currency;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceLocator.Chest
{
    [CreateAssetMenu(fileName = "ChestConfig", menuName = "ScriptableObjects/ChestConfig")]
    public class ChestConfig : ScriptableObject
    {
        public int minChestCount;
        public int maxChestCount;
        public List<ChestData> chests;
    }

    [System.Serializable]
    public class ChestData
    {
        public ChestType chestType;
        public float unlockTimeInMinutes;
        public CurrencyType chestUnlockCurrencyType; // currency Used to Unlock Chest before Timer
        public int chestUnlockMinutesSingleCurrency; //  minutes unlocked by a single currency
        public List<ChestRewards> rewards;
    }

    [System.Serializable]
    public class ChestRewards
    {
        public CurrencyType currencyType;
        public int minValue;
        public int maxValue;
    }
}