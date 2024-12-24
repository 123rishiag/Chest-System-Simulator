namespace ServiceLocator.Chest
{
    public enum ChestType
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
    public enum ChestState
    {
        Locked,
        Unlock_Queue,
        Unlocking,
        Unlocked,
        Collected
    }
}