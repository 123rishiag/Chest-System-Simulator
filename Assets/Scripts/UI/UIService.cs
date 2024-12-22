using System.Collections;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace ServiceLocator.UI
{
    public class UIService : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] public GameObject currencyPrefab;
        [SerializeField] public GameObject chestPrefab;

        [Header("UI Elements")]
        [SerializeField] private Button generateChestButton;

        [Header("Panels")]
        [SerializeField] public Transform currencyPanel;
        [SerializeField] public Transform chestSlotContentPanel;

        [Header("Chest Processing Elements")]
        [SerializeField] private GameObject chestProcessingPanel;
        [SerializeField] private Button chestProcessingActionOneButton;
        [SerializeField] private TMP_Text chestProcessingActionOneText;
        [SerializeField] private Button chestProcessingActionTwoButton;
        [SerializeField] private TMP_Text chestProcessingActionTwoText;
        [SerializeField] private Button chestProcessingCloseButton;

        [Header("Notification Popup Elements")]
        [SerializeField] private GameObject notificationPopupPanel;
        [SerializeField] private TMP_Text notificationPopupText;

        private void Start()
        {
            // Verifying all Inspector Elements
            ValidateReferences();
        }

        private void ValidateReferences()
        {
            if (generateChestButton == null)
            {
                Debug.LogWarning("Generate Chest Button reference is null!!");
                return;
            }
            if (currencyPanel == null || currencyPrefab == null)
            {
                Debug.LogWarning("Currency Panel or Prefab reference is null!!");
                return;
            }

            if (chestSlotContentPanel == null || chestPrefab == null)
            {
                Debug.LogWarning("Chest Slot Panel or Prefab reference is null!!");
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
        }

        public void AddGenerateChestButtonToListener(System.Action _onButtonClick)
        {
            generateChestButton.onClick.RemoveAllListeners();
            generateChestButton.onClick.AddListener(() =>
            {
                _onButtonClick?.Invoke();
            });
        }

        public void ConfigureButtons(System.Action _onButtonOneClick, System.Action _onButtonTwoClick, string _buttonOneText, string _buttonTwoText)
        {
            // Set the panel active
            chestProcessingPanel.SetActive(true);

            // Configure Button 1
            chestProcessingActionOneText.text = _buttonOneText;
            chestProcessingActionOneButton.onClick.RemoveAllListeners();
            chestProcessingActionOneButton.onClick.AddListener(() =>
            {
                _onButtonOneClick?.Invoke();
                chestProcessingPanel.SetActive(false);
            });

            // Configure Button 2
            chestProcessingActionTwoText.text = _buttonTwoText;
            chestProcessingActionTwoButton.onClick.RemoveAllListeners();
            chestProcessingActionTwoButton.onClick.AddListener(() =>
            {
                _onButtonTwoClick?.Invoke();
                chestProcessingPanel.SetActive(false);
            });

            // Configure Close Button
            chestProcessingCloseButton.onClick.RemoveAllListeners();
            chestProcessingCloseButton.onClick.AddListener(() =>
            {
                chestProcessingPanel.SetActive(false);
            });
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
    }
}