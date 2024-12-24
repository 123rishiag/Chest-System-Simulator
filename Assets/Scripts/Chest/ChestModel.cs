using ServiceLocator.Currency;

namespace ServiceLocator.Chest
{
    public class ChestModel
    {
        public ChestModel(ChestData _chestData)
        {
            ResetModel(_chestData);
        }

        public void ResetModel(ChestData _chestData)
        {
            // Setting Variables
            ChestData = _chestData;
            ChestState = ChestState.Locked;
            ChestType = _chestData.chestType;
            UnlockTimeInMinutes = _chestData.unlockTimeInMinutes;
            RemainingTimeInSeconds = UnlockTimeInMinutes * 60;
            ChestUnlockCurrencyType = _chestData.chestUnlockCurrencyType;
            IsCurrencyUsedToUnlock = false;
        }

        // Getters
        public ChestData ChestData { get; private set; }
        public ChestState ChestState { get; set; }
        public ChestType ChestType { get; private set; }
        public float UnlockTimeInMinutes { get; private set; }
        public float RemainingTimeInSeconds { get; set; }
        public CurrencyType ChestUnlockCurrencyType { get; private set; }
        public bool IsCurrencyUsedToUnlock { get; set; }
        public CurrencyModel ChestUnlockCurrencyModel { get; set; }
    }
}