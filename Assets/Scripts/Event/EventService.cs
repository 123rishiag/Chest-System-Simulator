using ServiceLocator.Currency;
using System;

namespace ServiceLocator.Event
{
    public class EventService
    {
        // Function to Get Currency Controller based on Currency Type
        public EventController<Func<CurrencyType, CurrencyController>> OnGetCurrencyControllerEvent { get; private set; }

        // Method to Add Currency based on Currency Type
        public EventController<Action<CurrencyType, int>> OnAddCurrencyEvent { get; private set; }

        // Method to Deduct Currency based on Currency Type
        public EventController<Action<CurrencyType, int>> OnDeductCurrencyEvent { get; private set; }

        public EventService()
        {
            OnGetCurrencyControllerEvent = new EventController<Func<CurrencyType, CurrencyController>>();
            OnAddCurrencyEvent = new EventController<Action<CurrencyType, int>>();
            OnDeductCurrencyEvent = new EventController<Action<CurrencyType, int>>();
        }
    }
}