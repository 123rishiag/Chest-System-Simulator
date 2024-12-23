using ServiceLocator.Event;

namespace ServiceLocator.UI
{
    public class UIService
    {
        // Private Variables
        private UIController uiController;

        // Private Services
        private EventService eventService;

        public UIService(UIView _uiCanvas, EventService _eventService)
        {
            // Setting Variables
            uiController = new UIController(_uiCanvas);

            // Setting Services
            eventService = _eventService;

            // Adding Listeners
            eventService.OnGetUIControllerEvent.AddListener(GetUIController);
        }

        ~UIService()
        {
            // Removing Listeners
            eventService.OnGetUIControllerEvent.RemoveListener(GetUIController);
        }

        // Getters
        private UIController GetUIController()
        {
            return uiController;
        }
    }
}