using ServiceLocator.Event;
using ServiceLocator.Utilities;
using UnityEngine;

namespace ServiceLocator.Chest
{
    public class ChestPool : GenericObjectPool<ChestController>
    {
        // Private Variables
        private ChestConfig chestConfig;
        private Transform parentTransform;

        // Private Services
        private EventService eventService;
        private ChestService chestService;

        public ChestPool(ChestConfig _chestConfig, Transform _parentTransform,
            EventService _eventService, ChestService _chestService)
        {
            // Setting Variables
            chestConfig = _chestConfig;
            parentTransform = _parentTransform;

            // Setting Services
            eventService = _eventService;
            chestService = _chestService;
        }

        public ChestController GetChest()
        {
            var item = GetItem();

            // Fetching Random Chest
            ChestData chestData = GetRandomChestData();
            if (chestData == null)
            {
                Debug.LogError("Random Chest Data is null!!");
                return null;
            }

            // Resetting Item Properties
            item.Reset(chestData, parentTransform.childCount - 1);

            return item;
        }

        protected override ChestController CreateItem()
        {
            // Fetching Random Chest
            ChestData chestData = GetRandomChestData();
            if (chestData == null)
            {
                Debug.LogError("Random Chest Data is null!!");
                return null;
            }

            return new ChestController(chestData, parentTransform, chestConfig.chestPrefab,
            eventService, chestService);
        }

        // Getters
        private ChestData GetRandomChestData()
        {
            // Calculating the total weight
            int totalWeight = chestConfig.chests.Count * (chestConfig.chests.Count + 1) / 2;

            // Generating a random number within the total weight
            int randomValue = Random.Range(1, totalWeight + 1);

            // Selecting a chest based on the random value
            int cumulativeWeight = 0;
            for (int i = 0; i < chestConfig.chests.Count; i++)
            {
                cumulativeWeight += chestConfig.chests.Count - i;
                if (randomValue <= cumulativeWeight)
                {
                    return chestConfig.chests[i];
                }
            }

            Debug.LogError("Failed to select a chest. Check weight logic!!");
            return null;
        }
    }
}