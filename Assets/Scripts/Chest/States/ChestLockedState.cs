using ServiceLocator.UI;

namespace ServiceLocator.Chest
{
    public class ChestLockedState<T> : IChestState where T : ChestController
    {
        public ChestController Owner { get; set; }
        private ChestGenericStateMachine<T> stateMachine;

        public ChestLockedState(ChestGenericStateMachine<T> _stateMachine) => stateMachine = _stateMachine;

        public void OnStateEnter()
        {
            // Setting Chest UI Elements
            Owner.GetChestView().chestUnlockCurrencyPanel.SetActive(true);

            // Setting Chest Button Interaction
            Owner.GetChestView().chestButton.interactable = true;

            // Setting Chest Buttons Listeners
            Owner.GetChestView().chestButton.onClick.RemoveAllListeners();
            Owner.GetChestView().chestButton.onClick.AddListener(() =>
            {
                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
                    () => StartTimer(),
                    () => Owner.UnlockWithCurrency(),
                    "Start Timer",
                    $"Unlock With {Owner.GetChestModel().ChestData.chestUnlockCurrencyType}s"
                );
            });
        }

        public void Update() { }

        private void StartTimer()
        {
            // If no other chests are unlocking, setting Chest State to "Unlocking"
            if (!Owner.GetChestService().IsAnyChestUnlocking())
            {
                stateMachine.ChangeState(ChestState.Unlocking);
                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ShowNotification("Timer Started!!");
            }
            // If any chest is unlocking, sending this chest to queue for unlock, setting Chest State to "Unlock_Queue"
            else
            {
                stateMachine.ChangeState(ChestState.Unlock_Queue);
                Owner.GetChestService().AddChestToQueue(Owner);
                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                    "Timer Already Running for another chest. Adding to Queue!!");
            }
        }
    }
}