using ServiceLocator.Event;
using ServiceLocator.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ServiceLocator.Chest
{
    public class ChestService
    {
        // Private Variables
        private ChestConfig chestConfig;
        private List<ChestController> chestControllers;
        private Queue<ChestController> chestUnlockQueue;

        // Private Services
        private EventService eventService;

        public ChestService(ChestConfig _chestConfig, EventService _eventService)
        {
            // Setting Variables
            chestConfig = _chestConfig;
            chestControllers = new List<ChestController>();
            chestUnlockQueue = new Queue<ChestController>();

            // Setting Services
            eventService = _eventService;

            // Validating References
            ValidateReferences();

            // Adding Chests
            CreateRandomChests();

            // Adding Listener to Chest Buttons
            eventService.OnGetUIControllerEvent.Invoke<UIController>().AddGenerateChestButtonToListener(CreateChest);
        }

        private void ValidateReferences()
        {
            if (chestConfig == null)
            {
                Debug.LogError("Chest Scriptable Object reference is null!!");
                return;
            }

            if (chestConfig.chestPrefab == null)
            {
                Debug.LogError("Chest Prefab reference is null!!");
                return;
            }
        }

        private void CreateRandomChests()
        {
            for (int i = 0; i < chestConfig.minChestCount; i++)
            {
                CreateChest();
            }
        }
        private void CreateChest()
        {
            // Fetching Random Chest
            ChestData chestData = GetRandomChestData();
            if (chestData == null)
            {
                Debug.LogError("Random Chest Data is null!!");
                return;
            }

            // Initializing a ChestController for random chest
            if (chestControllers.Count < chestConfig.maxChestCount)
            {
                var chestController = new ChestController(chestData,
                   eventService.OnGetUIControllerEvent.Invoke<UIController>().GetUIView().chestSlotContentPanel,
                    chestConfig.chestPrefab,
                    eventService, this);
                chestControllers.Add(chestController);
            }
            else
            {
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Can't add more chest. Max Limit is {chestConfig.maxChestCount}!!");
            }
        }
        private void RemoveChest(ChestController _chestController)
        {
            _chestController.Destroy();
            chestControllers.Remove(_chestController);
        }

        public void Update()
        {
            // Updating All Chests
            ProcessUpdateAllChests();

            // Processing Chests in Queue
            ProcessChestsInQueue();
        }
        private void ProcessUpdateAllChests()
        {
            for (int i = chestControllers.Count - 1; i >= 0; i--)
            {
                var chestController = chestControllers[i];
                chestController.Update();

                if (chestController.GetChestModel().ChestState == ChestState.Collected)
                {
                    RemoveChest(chestController);
                }
            }
        }
        private void ProcessChestsInQueue()
        {
            while (chestUnlockQueue.Count > 0)
            {
                var chestController = chestUnlockQueue.Peek();
                if (chestController.GetChestModel().ChestState != ChestState.Unlock_Queue)
                {
                    chestUnlockQueue.Dequeue();
                }
                else
                {
                    if (!IsAnyChestUnlocking())
                    {
                        chestController = chestUnlockQueue.Dequeue();
                        chestController.GetChestModel().ChestState = ChestState.Unlocking;
                    }
                    break;
                }
            }
        }

        // Other Functions
        public void AddChestToQueue(ChestController _chestController)
        {
            chestUnlockQueue.Enqueue(_chestController);
        }
        public bool IsAnyChestUnlocking()
        {
            return chestControllers.Any(chest => chest.GetChestModel().ChestState == ChestState.Unlocking);
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