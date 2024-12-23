using ServiceLocator.Chest;
using ServiceLocator.Currency;
using ServiceLocator.Event;
using ServiceLocator.UI;
using UnityEngine;

namespace ServiceLocator.Main
{
    public class GameService : MonoBehaviour
    {
        // Private Services
        private EventService eventService;
        private UIService uiService;
        private CurrencyService currencyService;
        private ChestService chestService;

        // Scriptable Objects
        [Header("Scriptable Objects")]
        [SerializeField] private UIView uiCanvas;
        [SerializeField] private CurrencyConfig currencyConfig;
        [SerializeField] private ChestConfig chestConfig;

        private void Start()
        {
            CreateServices();
        }

        private void CreateServices()
        {
            eventService = new EventService();
            uiService = new UIService(uiCanvas, eventService);
            currencyService = new CurrencyService(currencyConfig, eventService);
            chestService = new ChestService(chestConfig, eventService);
        }

        private void Update()
        {
            chestService.Update();
        }
    }
}