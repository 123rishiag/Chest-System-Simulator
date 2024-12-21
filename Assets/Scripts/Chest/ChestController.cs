using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestController
{
    private float unlockTimeInMinutes;
    private float remainingTimeInSeconds;

    private GameManager gameManager;

    private GameObject chestObject;
    private Button chestButton;

    private Image chestImage;
    private TMP_Text chestMessage1Text;
    private TMP_Text chestMessage2Text;

    public ChestState ChestState { get; private set; }
    public ChestType ChestType { get; private set; }
    public List<ChestRewards> Rewards { get; private set; }
    public ChestController(GameManager _gameManager, ChestData _chestData, Transform _parentTransform, GameObject _prefab)
    {
        gameManager = _gameManager;

        // Setting Chest Variables
        unlockTimeInMinutes = _chestData.unlockTimeInMinutes;
        remainingTimeInSeconds = unlockTimeInMinutes * 60;
        ChestState = ChestState.Locked;
        ChestType = _chestData.chestType;
        Rewards = _chestData.rewards;

        // Instantiating the prefab under the parent transform
        chestObject = Object.Instantiate(_prefab, _parentTransform);
        chestButton = chestObject.GetComponent<Button>();

        if (chestObject == null || chestButton == null)
        {
            Debug.LogError("Chest Prefab or Button not found!");
            return;
        }

        // Setting Chest UI
        chestImage = chestObject.GetComponent<Image>();
        if (chestImage == null)
        {
            Debug.LogError("Chest Image Field not found in prefab!!");
            return;
        }

        chestMessage1Text = chestObject.transform.Find("ChestMessageOnePanel").GetComponentInChildren<TMP_Text>();
        if (chestMessage1Text == null)
        {
            Debug.LogError("Chest Message 1 Text Field not found in prefab!!");
            return;
        }

        chestMessage2Text = chestObject.transform.Find("ChestMessageTwoPanel").GetComponentInChildren<TMP_Text>();
        if (chestMessage2Text == null)
        {
            Debug.LogError("Chest Message 2 Text Field not found in prefab!!");
            return;
        }

        UpdateUI();
    }

    public void Update()
    {
        PeformStateTransition();
        UpdateTimer();
        UpdateUI();
    }

    private void PeformStateTransition()
    {
        switch(ChestState)
        {
            case ChestState.Locked:
                chestButton.onClick.RemoveAllListeners();
                chestButton.onClick.AddListener(ProcessStartUnlocking);
                break;
            case ChestState.Unlocking:
                chestButton.onClick.RemoveAllListeners();
                ProcessUnlockChest();
                break;
            case ChestState.Unlocked:
                chestButton.onClick.RemoveAllListeners();
                chestButton.onClick.AddListener(ProcessCollectChest);
                break;
            default:
                break;
        }
    }
    private void UpdateTimer()
    {
        if (ChestState == ChestState.Unlocking)
        {
            remainingTimeInSeconds -= Time.deltaTime;
        }
    }
    private void UpdateUI()
    {
        chestImage.color = GetImageColor();
        chestMessage1Text.text = FormatTime(remainingTimeInSeconds);
        chestMessage2Text.text = $"{ChestType} : {ChestState}";
    }

    private void ProcessStartUnlocking()
    {
        if (!gameManager.IsAnyChestUnlocking())
        {
            ChestState = ChestState.Unlocking;
        }
    }
    private void ProcessUnlockChest()
    {
        if (remainingTimeInSeconds <= 0)
        {
            remainingTimeInSeconds = 0;
            ChestState = ChestState.Unlocked;
        }
    }

    private void ProcessCollectChest()
    {
        ChestState = ChestState.Collected;
    }

    private Color GetImageColor()
    {
        switch (ChestState)
        {
            case ChestState.Locked:
                return new Color(128, 128, 128, chestImage.color.a); // Grey
            case ChestState.Unlocking:
                return new Color(0, 0, 255, chestImage.color.a); // Blue
            case ChestState.Unlocked:
                return new Color(0, 255, 0, chestImage.color.a); // Green
            case ChestState.Collected:
                return new Color(255, 255, 0, chestImage.color.a); // Yellow
            default:
                return new Color(0, 0, 0, chestImage.color.a); // White
        }
    }

    private string FormatTime(float _timeInSeconds)
    {
        // Converting time from float to int
        int totalSeconds = Mathf.FloorToInt(_timeInSeconds);

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

    public void Destroy()
    {
        Object.Destroy(chestObject);
    }
}