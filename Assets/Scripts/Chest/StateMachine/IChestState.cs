namespace ServiceLocator.Chest
{
    public interface IChestState
    {
        public ChestController Owner { get; set; }
        public void OnStateEnter();
        public void Update();
    }
}