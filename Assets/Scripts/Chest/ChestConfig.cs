using UnityEngine;
using System.Collections.Generic;

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
    public List<ChestRewards> rewards;
}

[System.Serializable]
public class ChestRewards
{
    public CurrencyType currencyType;
    public int minValue;
    public int maxValue;
}