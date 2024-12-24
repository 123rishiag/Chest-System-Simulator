using System.Collections.Generic;
using System.Linq;

namespace ServiceLocator.Chest
{
    // Creating a generic state Machine, to handle Sub Chest Controllers if occurs in future
    // if based on Chest Types like Common, Rare, Epic, Legendary etc can only transition to certain states
    public class ChestGenericStateMachine<T> where T : ChestController
    {
        protected T owner;
        protected IChestState currentState;
        protected Dictionary<ChestState, IChestState> ChestStates = new Dictionary<ChestState, IChestState>();

        public ChestGenericStateMachine(T _owner) => owner = _owner;

        public void Update() => currentState?.Update();

        public ChestState GetCurrentState()
        {
            return ChestStates.Keys.ToList().Find(key => ChestStates[key] == currentState);
        }

        protected void ChangeState(IChestState _newState)
        {
            currentState = _newState;
            currentState?.OnStateEnter();
        }

        public void ChangeState(ChestState _newState) => ChangeState(ChestStates[_newState]);

        protected void SetOwner()
        {
            foreach (IChestState _chestState in ChestStates.Values)
            {
                _chestState.Owner = owner;
            }
        }
    }
}