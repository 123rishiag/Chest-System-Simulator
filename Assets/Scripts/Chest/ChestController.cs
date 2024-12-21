using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestController
{
    private float unlockTimeInMinutes;
    private TMP_Text chestMessage1Text;
    private TMP_Text chestMessage2Text;

    public ChestType ChestType { get; private set; }
    public List<ChestRewards> Rewards { get; private set; }
    public ChestController(ChestData _chestData, Transform _parentTransform, GameObject _prefab)
    {
        // Setting Chest Variables
        unlockTimeInMinutes = _chestData.unlockTimeInMinutes;
        ChestType = _chestData.chestType;
        Rewards = _chestData.rewards;

        // Instantiating the prefab under the parent transform
        GameObject instance = Object.Instantiate(_prefab, _parentTransform);

        if (instance == null)
        {
            Debug.LogError("Chest Prefab not found!");
            return;
        }

        // Setting Chest UI
        chestMessage1Text = instance.transform.Find("ChestMessageOnePanel").GetComponentInChildren<TMP_Text>();
        if (chestMessage1Text == null)
        {
            Debug.LogError("Chest Message 1 Text Field not found in prefab!!");
            return;
        }
        else
        {
            chestMessage1Text.text = FormatTime(unlockTimeInMinutes);
        }

        chestMessage2Text = instance.transform.Find("ChestMessageTwoPanel").GetComponentInChildren<TMP_Text>();
        if (chestMessage2Text == null)
        {
            Debug.LogError("Chest Message 2 Text Field not found in prefab!!");
            return;
        }
        else
        {
            chestMessage2Text.text = ChestType.ToString();
        }
    }

    private string FormatTime(float _timeInMinutes)
    {
        // Converting time into seconds
        int totalSeconds = Mathf.FloorToInt(_timeInMinutes * 60);

        // Determining the time range
        switch (totalSeconds)
        {
            case < 60:
                // Less than 1 minute
                return $"{totalSeconds}sec";

            case < 3600:
                // Less than 1 hour
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                return $"{minutes}min {seconds}sec";

            case < 86400:
                // Less than 1 day
                int hours = totalSeconds / 3600;
                minutes = (totalSeconds % 3600) / 60;
                return $"{hours}H {minutes}min";

            default:
                // 1 day or more
                int days = totalSeconds / 86400;
                hours = (totalSeconds % 86400) / 3600;
                return $"{days}D {hours}H";
        }
    }
}