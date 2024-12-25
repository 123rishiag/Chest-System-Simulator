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
        [SerializeField] private Image chestUnlockCurrencyPanelImage;
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

            if (chestUnlockCurrencyPanelImage == null)
            {
                Debug.LogError("Chest Unlock Currency Panel Image not found in prefab!!");
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

        public void ResetView(int _index)
        {
            transform.SetSiblingIndex(_index);
        }
        private void Show()
        {
            gameObject.SetActive(true);
        }
        private void Hide()
        {
            StartCoroutine(HideObject(4f));
        }
        private IEnumerator HideObject(float _timeInSeconds)
        {
            yield return new WaitForSeconds(_timeInSeconds);
            gameObject.SetActive(false);
        }

        // Getters
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
        // Defining colors as static fields for readability and maintainability
        private static readonly Color Grey = new Color(0.56f, 0.66f, 0.78f); // Locked
        private static readonly Color Purple = new Color(0.61f, 0.35f, 0.71f); // Unlock Queue
        private static readonly Color Blue = new Color(0.36f, 0.68f, 0.89f);  // Unlocking
        private static readonly Color Green = new Color(0.35f, 0.84f, 0.55f); // Unlocked
        private static readonly Color Yellow = new Color(1f, 0.84f, 0f); // Collected
        private static readonly Color White = new Color(1f, 1f, 1f); // Default fallback
        private Color ApplyAlpha(Color _baseColor, float _alpha)
        {
            // To apply alpha dynamically
            return new Color(_baseColor.r, _baseColor.g, _baseColor.b, _alpha);
        }
        private Color GetContrastingTextColor(Color _backgroundColor)
        {
            // Calculating brightness (accounting for perceived intensity of RGB components)
            float brightness = (_backgroundColor.r * 0.299f + _backgroundColor.g * 0.587f + _backgroundColor.b * 0.114f);

            // Adjusting the brightness based on alpha (transparent colors are treated as darker)
            brightness *= _backgroundColor.a;

            // Determining contrasting color: dark backgrounds get white text, bright backgrounds get black text
            return brightness > 0.5f ? Color.black : Color.white;
        }
        private Color GetImageColor()
        {
            switch (chestController.GetChestStateMachine().GetCurrentState())
            {
                case ChestState.Locked:
                    return Grey;
                case ChestState.Unlock_Queue:
                    return Purple;
                case ChestState.Unlocking:
                    return Blue;
                case ChestState.Unlocked:
                    return Green;
                case ChestState.Collected:
                    return Yellow;
                default:
                    return White;
            }
        }
        private Color GetGradientColor()
        {
            // Applying a slight gradient effect for some states
            Color baseColor = GetImageColor();
            return Color.Lerp(baseColor, Color.white, 0.2f); // Blend with white for a lighter gradient
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

            // Applying the background color to the chest image with alpha
            chestImage.color = ApplyAlpha(GetGradientColor(), chestImage.color.a);

            // Setting text content and adjusting text color for readability
            chestMessageOneText.text = GetTimeString(chestController.GetChestModel().RemainingTimeInSeconds);
            chestMessageOneText.color = GetContrastingTextColor(chestImage.color);

            chestMessageTwoText.text = $"{chestController.GetChestModel().ChestType} : " +
                $"{chestController.GetChestStateMachine().GetCurrentState()}";
            chestMessageTwoText.color = GetContrastingTextColor(chestImage.color);

            // Setting the unlock currency panel color
            chestUnlockCurrencyPanelImage.color = ApplyAlpha(GetGradientColor(), chestUnlockCurrencyPanelImage.color.a);

            // Setting the unlock currency text and color
            chestUnlockCurrencyText.text = chestController.GetCurrencyRequiredToUnlock().ToString();
            chestUnlockCurrencyText.color = GetContrastingTextColor(chestUnlockCurrencyPanelImage.color);

            // Setting the unlock currency image and its color
            chestUnlockCurrencyImage.sprite = currencyData.currencyImage;
            chestUnlockCurrencyImage.color = ApplyAlpha(currencyData.imageColor, chestUnlockCurrencyImage.color.a);


            // Managing view visibility based on the state
            if (chestController.GetChestStateMachine().GetCurrentState() == ChestState.Collected)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}