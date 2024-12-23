using ServiceLocator.Chest;
using ServiceLocator.Currency;
using ServiceLocator.UI;
using UnityEngine;

namespace ServiceLocator.Main
{
    public class GameService : MonoBehaviour
    {
        // Private Services
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
            InjectDependencies();
        }
        private void CreateServices()
        {
            uiService = new UIService(uiCanvas);
            currencyService = new CurrencyService(currencyConfig);
            chestService = new ChestService(chestConfig);
        }
        private void InjectDependencies()
        {
            currencyService.Init(uiService);
            chestService.Init(uiService, currencyService);
        }

        private void Update()
        {
            chestService.Update();
        }
    }
}