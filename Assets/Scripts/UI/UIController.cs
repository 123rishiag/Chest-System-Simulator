namespace ServiceLocator.UI
{
    public class UIController
    {
        // Private Variables
        private UIView uiView;

        public UIController(UIView _uiCanvas)
        {
            // Setting Variables
            uiView = _uiCanvas.GetComponent<UIView>();

            // Verifying all Inspector Elements
            uiView.ValidateReferences();
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

            // Configure Button 1
            uiView.chestProcessingActionOneButton.onClick.RemoveAllListeners();
            uiView.chestProcessingActionOneButton.onClick.AddListener(() =>
            {
                _onButtonOneClick?.Invoke();
                uiView.chestProcessingPanel.SetActive(false);
            });

            // Configure Button 2
            uiView.chestProcessingActionTwoButton.onClick.RemoveAllListeners();
            uiView.chestProcessingActionTwoButton.onClick.AddListener(() =>
            {
                _onButtonTwoClick?.Invoke();
                uiView.chestProcessingPanel.SetActive(false);
            });

            // Configure Close Button
            uiView.chestProcessingCloseButton.onClick.RemoveAllListeners();
            uiView.chestProcessingCloseButton.onClick.AddListener(() =>
            {
                uiView.chestProcessingPanel.SetActive(false);
            });
        }

        public void ShowNotification(string _text)
        {
            uiView.ShowNotification(_text);
        }

        // Getters
        public UIView GetUIView()
        {
            return uiView;
        }
    }
}