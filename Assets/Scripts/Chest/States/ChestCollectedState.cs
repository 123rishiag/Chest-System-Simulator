namespace ServiceLocator.Chest
{
    public class ChestCollectedState<T> : IChestState where T : ChestController
    {
        public ChestController Owner { get; set; }
        private ChestGenericStateMachine<T> stateMachine;

        public ChestCollectedState(ChestGenericStateMachine<T> _stateMachine) => stateMachine = _stateMachine;

        public void OnStateEnter()
        {
            // Setting Chest UI Elements
            Owner.GetChestView().chestUnlockCurrencyPanel.SetActive(false);

            // Setting Chest Button Interaction
            Owner.GetChestView().chestButton.interactable = false;
        }

        public void Update() { }
    }
}