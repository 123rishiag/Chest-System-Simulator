using ServiceLocator.Currency;
using ServiceLocator.UI;
using System;

namespace ServiceLocator.Event
{
    public class EventService
    {
        // Function to return UIController
        public EventController<Func<UIController>> OnGetUIControllerEvent { get; private set; }

        // Function to Get Currency Controller based on Currency Type
        public EventController<Func<CurrencyType, CurrencyController>> OnGetCurrencyControllerEvent { get; private set; }

        public EventService()
        {
            OnGetUIControllerEvent = new EventController<Func<UIController>>();
            OnGetCurrencyControllerEvent = new EventController<Func<CurrencyType, CurrencyController>>();
        }
    }
}