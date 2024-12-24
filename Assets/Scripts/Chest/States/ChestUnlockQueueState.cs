using ServiceLocator.UI;

namespace ServiceLocator.Chest
{
    public class ChestUnlockQueueState<T> : IChestState where T : ChestController
    {
        public ChestController Owner { get; set; }
        private ChestGenericStateMachine<T> stateMachine;

        public ChestUnlockQueueState(ChestGenericStateMachine<T> _stateMachine) => stateMachine = _stateMachine;

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
                    () => Owner.UnlockWithCurrency(),
                    () => { },
                    $"Unlock With {Owner.GetChestModel().ChestData.chestUnlockCurrencyType}s",
                    "Cancel"
                );
            });
        }

        public void Update() { }
    }
}