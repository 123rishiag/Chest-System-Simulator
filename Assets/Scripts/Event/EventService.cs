using ServiceLocator.Currency;
using ServiceLocator.Sound;
using ServiceLocator.UI;
using System;

namespace ServiceLocator.Event
{
    public class EventService
    {
        // Method to Play a particular Sound Effect - Sound Service
        public EventController<Action<SoundType>> OnPlaySoundEffectEvent { get; private set; }

        // Function to return UIController - UI Service
        public EventController<Func<UIController>> OnGetUIControllerEvent { get; private set; }

        // Function to Get Currency Controller based on Currency Type - Currency Service
        public EventController<Func<CurrencyType, CurrencyController>> OnGetCurrencyControllerEvent { get; private set; }

        public EventService()
        {
            OnPlaySoundEffectEvent = new EventController<Action<SoundType>>();
            OnGetUIControllerEvent = new EventController<Func<UIController>>();
            OnGetCurrencyControllerEvent = new EventController<Func<CurrencyType, CurrencyController>>();
        }
    }
}