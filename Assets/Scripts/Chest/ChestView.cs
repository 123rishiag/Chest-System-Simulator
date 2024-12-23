using ServiceLocator.Currency;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ServiceLocator.Chest
{
    public class ChestView : MonoBehaviour
    {
        // UI Elements
        [Header("UI Elements")]
        [SerializeField] public Button chestButton;
        [SerializeField] private Image chestImage;
        [SerializeField] private TMP_Text chestMessageOneText;
        [SerializeField] private TMP_Text chestMessageTwoText;
        [SerializeField] public GameObject chestUnlockCurrencyPanel;
        [SerializeField] TMP_Text chestUnlockCurrencyText;
        [SerializeField] private Image chestUnlockCurrencyImage;

        // Private Variables
        private ChestController chestController;

        private void ValidateReferences()
        {
            if (chestButton == null)
            {
                Debug.LogError("Chest Button not found!!");
                return;
            }

            if (chestImage == null)
            {
                Debug.LogError("Chest Image Field not found in prefab!!");
                return;
            }

            if (chestMessageOneText == null)
            {
                Debug.LogError("Chest Message 1 Text Field not found in prefab!!");
                return;
            }

            if (chestMessageTwoText == null)
            {
                Debug.LogError("Chest Message 2 Text Field not found in prefab!!");
                return;
            }

            if (chestUnlockCurrencyPanel == null)
            {
                Debug.LogError("Chest Unlock Currency Panel not found in prefab!!");
                return;
            }

            if (chestUnlockCurrencyText == null)
            {
                Debug.LogError("Chest Unlock Currency Text Field not found in prefab!!");
                return;
            }

            if (chestUnlockCurrencyImage == null)
            {
                Debug.LogError("Chest Unlock Currency Image Field not found in prefab!!");
                return;
            }
        }

        public void Destroy()
        {
            StartCoroutine(DestroyObject(4f));
        }
        private IEnumerator DestroyObject(float _timeInSeconds)
        {
            yield return new WaitForSeconds(_timeInSeconds);
            Object.Destroy(gameObject);
        }

        // Getters
        private Color GetImageColor()
        {
            switch (chestController.GetChestModel().ChestState)
            {
                case ChestState.Locked:
                    return new Color(128, 128, 128, chestImage.color.a); // Grey
                case ChestState.Unlock_Queue:
                    return new Color(128, 0, 128, chestImage.color.a); // Purple
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
        private string GetTimeString(float _timeInSeconds)
        {
            // Converting time from float to int
            int totalSeconds = Mathf.FloorToInt(_timeInSeconds);
            int days, hours, minutes, seconds = 0;

            // Determining the time range
            switch (totalSeconds)
            {
                case < 60:
                    // Less than 1 minute
                    return $"{totalSeconds}sec";

                case < 3600:
                    // Less than 1 hour
                    minutes = totalSeconds / 60;
                    seconds = totalSeconds % 60;
                    return $"{minutes}min {seconds}sec";

                case < 86400:
                    // Less than 1 day
                    hours = totalSeconds / 3600;
                    minutes = (totalSeconds % 3600) / 60;
                    seconds = totalSeconds % 60;
                    return $"{hours}H {minutes}min {seconds}sec";

                default:
                    // 1 day or more
                    days = totalSeconds / 86400;
                    hours = (totalSeconds % 86400) / 3600;
                    minutes = (totalSeconds % 3600) / 60;
                    return $"{days}D {hours}H {minutes}min";
            }
        }

        // Setters
        public void SetViewProperties(ChestController _chestController)
        {
            // Setting Variables
            chestController = _chestController;

            // Validating References
            ValidateReferences();
        }
        public void UpdateUI()
        {
            CurrencyData currencyData = chestController.GetChestModel().ChestUnlockCurrencyModel.CurrencyData;
            chestImage.color = GetImageColor();
            chestMessageOneText.text = GetTimeString(chestController.GetChestModel().RemainingTimeInSeconds);
            chestMessageTwoText.text = $"{chestController.GetChestModel().ChestType} : {chestController.GetChestModel().ChestState}";
            chestUnlockCurrencyText.text = chestController.GetCurrencyRequiredToUnlock().ToString();
            chestUnlockCurrencyImage.sprite = currencyData.currencyImage;
            chestUnlockCurrencyImage.color = new Color(currencyData.imageColor.r, currencyData.imageColor.g,
                currencyData.imageColor.b, chestUnlockCurrencyImage.color.a);
        }
    }
}