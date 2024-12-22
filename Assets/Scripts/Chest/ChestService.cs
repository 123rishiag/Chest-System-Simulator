using ServiceLocator.Currency;
using ServiceLocator.UI;
using System.Collections.Generic;
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
        private UIService uiService;
        private CurrencyService currencyService;

        public ChestService(ChestConfig _chestConfig)
        {
            // Setting Variables
            chestConfig = _chestConfig;
            chestControllers = new List<ChestController>();
            chestUnlockQueue = new Queue<ChestController>();

            // Validating References
            ValidateReferences();
        }

        public void Init(UIService _uiService, CurrencyService _currencyService)
        {
            // Setting Services
            uiService = _uiService;
            currencyService = _currencyService;

            // Adding Chests
            CreateRandomChests();

            // Adding Listener to Chest Buttons
            uiService.AddGenerateChestButtonToListener(CreateChest);
        }

        private void ValidateReferences()
        {
            if (chestConfig == null)
            {
                Debug.LogWarning("Chest Scriptable Object reference is null!");
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
            ChestData chestData = GetRandomChest();
            if (chestData == null)
            {
                Debug.Log("Random Chest Data is null!");
                return;
            }

            // Initializing a ChestController for random chest
            if (chestControllers.Count < chestConfig.maxChestCount)
            {
                var chestController = new ChestController(chestData, uiService.chestSlotContentPanel, uiService.chestPrefab,
                    uiService, currencyService, this);
                chestControllers.Add(chestController);
            }
        }
        private void RemoveChest(ChestController _chestController)
        {
            _chestController.Destroy();
            chestControllers.Remove(_chestController);
        }

        public void Update()
        {
            // Updating Chests
            for (int i = chestControllers.Count - 1; i >= 0; i--)
            {
                var chestController = chestControllers[i];
                chestController.Update();

                if (chestController.ChestState == ChestState.Collected)
                {
                    RemoveChest(chestController);
                }
            }

            // Processing Chests in Queue
            ProcessChestsInQueue();
        }
        private void ProcessChestsInQueue()
        {
            while (chestUnlockQueue.Count > 0)
            {
                var chestController = chestUnlockQueue.Peek();
                if (chestController.ChestState != ChestState.Unlock_Queue)
                {
                    chestUnlockQueue.Dequeue();
                }
                else
                {
                    if (!IsAnyChestUnlocking())
                    {
                        chestController = chestUnlockQueue.Dequeue();
                        chestController.ChestState = ChestState.Unlocking;
                    }
                    break;
                }
            }
        }

        public bool IsAnyChestUnlocking()
        {
            foreach (var chestController in chestControllers)
            {
                if (chestController.ChestState == ChestState.Unlocking)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddChestToQueue(ChestController _chestController)
        {
            chestUnlockQueue.Enqueue(_chestController);
        }

        // Getters
        private ChestData GetRandomChest()
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

            Debug.LogError("Failed to select a chest. Check weight logic!");
            return null;
        }
    }
}