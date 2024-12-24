using ServiceLocator.Chest;
using ServiceLocator.Currency;
using ServiceLocator.Event;
using ServiceLocator.Sound;
using ServiceLocator.UI;
using UnityEngine;

namespace ServiceLocator.Main
{
    public class GameService : MonoBehaviour
    {
        // Private Variables
        [Header("Core Components")]
        [SerializeField] private SoundConfig soundConfig;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgSource;
        [SerializeField] private UIView uiCanvas;
        [SerializeField] private CurrencyConfig currencyConfig;
        [SerializeField] private ChestConfig chestConfig;

        // Private Services
        private EventService eventService;
        private SoundService soundService;
        private UIService uiService;
        private CurrencyService currencyService;
        private ChestService chestService;

        private void Start()
        {
            CreateServices();
        }

        private void CreateServices()
        {
            eventService = new EventService();
            soundService = new SoundService(soundConfig, sfxSource, bgSource, eventService);
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