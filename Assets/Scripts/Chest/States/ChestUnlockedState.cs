using ServiceLocator.Currency;
using ServiceLocator.UI;
using UnityEngine;

namespace ServiceLocator.Chest
{
    public class ChestUnlockedState<T> : IChestState where T : ChestController
    {
        public ChestController Owner { get; set; }
        private ChestGenericStateMachine<T> stateMachine;

        public ChestUnlockedState(ChestGenericStateMachine<T> _stateMachine) => stateMachine = _stateMachine;

        public void OnStateEnter()
        {
            // Setting Chest UI Elements
            Owner.GetChestView().chestUnlockCurrencyPanel.SetActive(false);

            // Setting Chest Button Interaction
            Owner.GetChestView().chestButton.interactable = true;

            // Setting Chest Buttons Listeners
            Owner.GetChestView().chestButton.onClick.RemoveAllListeners();
            Owner.GetChestView().chestButton.onClick.AddListener(() =>
            {
                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ConfigureButtons(
                    () => ProcessCollectChest(),
                    () => ProcessUndoCurrencyPurchase(),
                    "Collect Chest",
                    Owner.GetUndoCurrencyPurchaseString()
                );
            });
        }

        public void Update() { }

        private void ProcessCollectChest()
        {
            // Setting Chest State to "Collected"
            stateMachine.ChangeState(ChestState.Collected);

            // Adding Currency Rewards in our stock and fetching all rewards with type and quantity to show notification
            string rewardText = string.Empty;
            foreach (var reward in Owner.GetChestModel().ChestData.rewards)
            {
                int rewardRandomValue = Random.Range(reward.minValue, reward.maxValue + 1);
                rewardText += $" {rewardRandomValue} {reward.currencyType}s";

                // Adding Currency
                Owner.GetEventService().OnGetCurrencyControllerEvent.Invoke<CurrencyController>(
                    reward.currencyType).AddCurrency(rewardRandomValue);
            }

            // Showing All Rewards Notification
            Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                $"Chest Collected!! You gained {rewardText}!!");
        }

        private void ProcessUndoCurrencyPurchase()
        {
            // Checking if currency was used to unlock, if yes, setting Chest State to "Locked",
            // reverting the "IsCurrencyUsedToUnlock" flag and re
            if (Owner.GetChestModel().IsCurrencyUsedToUnlock)
            {
                stateMachine.ChangeState(ChestState.Locked);
                Owner.GetChestModel().IsCurrencyUsedToUnlock = false;

                int currencyRequired = Owner.GetCurrencyRequiredToUnlock();

                // Adding the currency back
                Owner.GetEventService().OnGetCurrencyControllerEvent.Invoke<CurrencyController>(
                    Owner.GetChestModel().ChestUnlockCurrencyType).AddCurrency(currencyRequired);

                Owner.GetEventService().OnGetUIControllerEvent.Invoke<UIController>().ShowNotification(
                    $"Reverted {Owner.GetChestModel().ChestUnlockCurrencyType} chest unlock. " +
                    $"You gained {currencyRequired} {Owner.GetChestModel().ChestUnlockCurrencyType}s!!");
            }
        }
    }
}