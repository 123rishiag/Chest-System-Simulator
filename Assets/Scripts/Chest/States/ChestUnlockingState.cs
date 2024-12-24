using ServiceLocator.Sound;
using ServiceLocator.UI;
using UnityEngine;

namespace ServiceLocator.Chest
{
    public class ChestUnlockingState<T> : IChestState where T : ChestController
    {
        public ChestController Owner { get; set; }
        private ChestGenericStateMachine<T> stateMachine;

        public ChestUnlockingState(ChestGenericStateMachine<T> _stateMachine) => stateMachine = _stateMachine;

        public void OnStateEnter()
        {
            // Setting Chest UI Elements
            Owner.GetChestView().chestUnlockCurrencyPanel.SetActive(true);

            // Setting Chest Button Interaction
            Owner.GetChestView().chestButton.interactable = true;

            // Setting Chest Buttons Listeners
            Owner.GetChestView().chestButton.onClick.RemoveAllListeners();
            Owner.GetChestView().chestButton.onClick.RemoveAllListeners();
            Owner.GetChestView().chestButton.onClick.AddListener(() =>
            {
                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
                    () => Owner.UnlockWithCurrency(),
                    () => ProcessStopUnlocking(),
                    $"Unlock With {Owner.GetChestModel().ChestData.chestUnlockCurrencyType}s",
                    "Stop Unlocking"
                );
            });
        }

        public void Update()
        {
            // Running Timer
            Owner.GetChestModel().RemainingTimeInSeconds -= Time.deltaTime;

            // If Timer is completed, setting Chest State to "Unlocked"
            if (Owner.GetChestModel().RemainingTimeInSeconds <= 0)
            {
                stateMachine.ChangeState(ChestState.Unlocked);
                Owner.GetChestModel().RemainingTimeInSeconds = 0;
                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                    "Timer Completed. Chest Unlocked!!");
            }
        }

        private void ProcessStopUnlocking()
        {
            // To Stop Unlocking, setting Chest State to "Locked"
            stateMachine.ChangeState(ChestState.Locked);
            Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                $"Chest stopped Unlocking!!");

            // Playing Sound
            Owner.GetEventService().OnPlaySoundEffectEvent.Invoke(SoundType.ChestLocked);
        }
    }
}