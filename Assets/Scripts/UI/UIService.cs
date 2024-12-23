namespace ServiceLocator.UI
{
    public class UIService
    {
        // Private Variables
        private UIController uiController;

        public UIService(UIView _uiCanvas)
        {
            // Setting Variables
            uiController = new UIController(_uiCanvas);
        }

        // Getters
        public UIController GetUIController()
        {
            return uiController;
        }
    }
}