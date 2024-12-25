using ServiceLocator.Event;
using ServiceLocator.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace ServiceLocator.UI
{
    public class UIController
    {
        // Private Variables
        private UIView uiView;

        // Private Services
        private EventService eventService;

        public UIController(UIView _uiCanvas, EventService _eventService)
        {
            // Setting Variables
            uiView = _uiCanvas.GetComponent<UIView>();

            // Setting Services
            eventService = _eventService;

            // Validating References
            uiView.ValidateReferences();
            SetMainMenuButtons();
        }

        public void AddGenerateChestButtonToListener(System.Action _onButtonClick)
        {
            uiView.generateChestButton.onClick.RemoveAllListeners();
            uiView.generateChestButton.onClick.AddListener(() =>
            {
                _onButtonClick?.Invoke();
            });
        }

        public void ConfigureButtons(System.Action _onButtonOneClick, System.Action _onButtonTwoClick, string _buttonOneText, string _buttonTwoText)
        {
            // Setting Buttons Text
            uiView.ConfigureButtonsText(_buttonOneText, _buttonTwoText);

            // Set the panel active
            uiView.chestProcessingPanel.SetActive(true);

            // Playing Sound
            eventService.OnPlaySoundEffectEvent.Invoke(SoundType.ConfirmationPopup);

            // Configure Button 1
            uiView.chestProcessingActionOneButton.onClick.RemoveAllListeners();
            uiView.chestProcessingActionOneButton.onClick.AddListener(() =>
            {
                _onButtonOneClick?.Invoke();
                uiView.chestProcessingPanel.SetActive(false);
                eventService.OnPlaySoundEffectEvent.Invoke(SoundType.ButtonClick);
            });

            // Configure Button 2
            uiView.chestProcessingActionTwoButton.onClick.RemoveAllListeners();
            uiView.chestProcessingActionTwoButton.onClick.AddListener(() =>
            {
                _onButtonTwoClick?.Invoke();
                uiView.chestProcessingPanel.SetActive(false);
                eventService.OnPlaySoundEffectEvent.Invoke(SoundType.ButtonClick);
            });

            // Configure Close Button
            uiView.chestProcessingCloseButton.onClick.RemoveAllListeners();
            uiView.chestProcessingCloseButton.onClick.AddListener(() =>
            {
                uiView.chestProcessingPanel.SetActive(false);
                eventService.OnPlaySoundEffectEvent.Invoke(SoundType.ButtonClick);
            });
        }

        public void ShowNotification(string _text)
        {
            uiView.ShowNotification(_text);
        }

        private void SetMainMenuButtons()
        {
            // Fetching UI Elements
            Button startButton;
            Button quitButton;

            (startButton, quitButton) = uiView.CreateMainMenuButtons();

            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(() =>
                {
                    eventService.OnPlaySoundEffectEvent.Invoke(SoundType.ButtonClick);
                    uiView.mainMenuPanel.SetActive(false);
                }
                );
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(() =>
                {
                    eventService.OnPlaySoundEffectEvent.Invoke(SoundType.ButtonClick);
                    Application.Quit();
                }
                );
            }
        }

        // Getters
        public UIView GetUIView()
        {
            return uiView;
        }
    }
}