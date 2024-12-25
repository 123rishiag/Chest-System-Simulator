using System.Collections;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace ServiceLocator.UI
{
    public class UIView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] public Button generateChestButton;

        [Header("Panels")]
        [SerializeField] public Transform currencyPanel;
        [SerializeField] public Transform chestSlotContentPanel;

        [Header("Chest Processing Elements")]
        [SerializeField] public GameObject chestProcessingPanel;
        [SerializeField] public Button chestProcessingActionOneButton;
        [SerializeField] private TMP_Text chestProcessingActionOneText;
        [SerializeField] public Button chestProcessingActionTwoButton;
        [SerializeField] private TMP_Text chestProcessingActionTwoText;
        [SerializeField] public Button chestProcessingCloseButton;

        [Header("Notification Popup Elements")]
        [SerializeField] private GameObject notificationPopupPanel;
        [SerializeField] private TMP_Text notificationPopupText;

        [Header("Main Menu Elements")]
        [SerializeField] public GameObject mainMenuPanel;
        [SerializeField] private GameObject mainMenuStartButton;
        [SerializeField] private GameObject mainMenuQuitButton;


        public void ValidateReferences()
        {
            if (generateChestButton == null)
            {
                Debug.LogError("Generate Chest Button reference is null!!");
                return;
            }

            if (currencyPanel == null)
            {
                Debug.LogError("Currency Panel reference is null!!");
                return;
            }

            if (chestSlotContentPanel == null)
            {
                Debug.LogError("Chest Slot Panel reference is null!!");
                return;
            }

            if (chestProcessingPanel == null)
            {
                Debug.LogError("Chest Processing Panel not found!!");
                return;
            }

            if (chestProcessingActionOneButton == null || chestProcessingActionOneText == null)
            {
                Debug.LogError("Chest Processing Action One Button or Text Field not found in panel!!");
                return;
            }

            if (chestProcessingActionTwoButton == null || chestProcessingActionTwoText == null)
            {
                Debug.LogError("Chest Processing Action Two Button or Text Field not found in panel!!");
                return;
            }

            if (chestProcessingCloseButton == null)
            {
                Debug.LogError("Chest Processing Close Button not found in panel!!");
                return;
            }

            if (notificationPopupPanel == null || notificationPopupText == null)
            {
                Debug.LogError("Notification Popup Panel or Text Field reference is null!!");
                return;
            }

            if (mainMenuPanel == null || mainMenuStartButton == null || mainMenuQuitButton == null)
            {
                Debug.LogError("Main Menu Panel or Start or Quit Button Field reference is null!!");
                return;
            }
        }

        public void ConfigureButtonsText(string _buttonOneText, string _buttonTwoText)
        {
            // Setting Button 1 Text
            chestProcessingActionOneText.text = _buttonOneText;

            // Setting Button 2 Text
            chestProcessingActionTwoText.text = _buttonTwoText;
        }

        public void ShowNotification(string _text)
        {
            notificationPopupText.text = _text;
            StartCoroutine(PopupNotification(2f));
        }

        private IEnumerator PopupNotification(float _timeInSeconds)
        {
            notificationPopupPanel.SetActive(true);
            yield return new WaitForSeconds(_timeInSeconds);
            notificationPopupPanel.SetActive(false);
        }

        public (Button, Button) CreateMainMenuButtons()
        {
            // Fetching UI Elements
            Button startButton = mainMenuStartButton.GetComponent<Button>();
            Button quitButton = mainMenuQuitButton.GetComponent<Button>();

            return (startButton, quitButton);
        }
    }
}