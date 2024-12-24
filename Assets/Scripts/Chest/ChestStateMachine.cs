namespace ServiceLocator.Chest
{
    public class ChestStateMachine : ChestGenericStateMachine<ChestController>
    {
        public ChestStateMachine(ChestController _owner) : base(_owner)
        {
            owner = _owner;
            CreateStates();
            SetOwner();
        }

        private void CreateStates()
        {
            ChestStates.Add(ChestState.Locked, new ChestLockedState<ChestController>(this));
            ChestStates.Add(ChestState.Unlock_Queue, new ChestUnlockQueueState<ChestController>(this));
            ChestStates.Add(ChestState.Unlocking, new ChestUnlockingState<ChestController>(this));
            ChestStates.Add(ChestState.Unlocked, new ChestUnlockedState<ChestController>(this));
            ChestStates.Add(ChestState.Collected, new ChestCollectedState<ChestController>(this));
        }
    }
}