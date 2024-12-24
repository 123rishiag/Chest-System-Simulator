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
        private ChestPool chestPool;
        private Queue<ChestController> chestUnlockQueue;

        // Private Services
        private EventService eventService;

        public ChestService(ChestConfig _chestConfig, EventService _eventService)
        {
            // Setting Variables
            chestConfig = _chestConfig;
            chestPool = new ChestPool(chestConfig,
                   _eventService.OnGetUIControllerEvent.Invoke<UIController>().GetUIView().chestSlotContentPanel,
                    _eventService, this);
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
            // Initializing a ChestController for random chest
            if (chestPool.pooledItems.Count(item => item.isUsed) < chestConfig.maxChestCount)
            {
                chestPool.GetChest();
            }
            else
            {
                eventService.OnGetUIControllerEvent.Invoke<UIController>().ShowNotification($"Can't add more chest. Max Limit is {chestConfig.maxChestCount}!!");
            }
        }
        private void ReturnChestToPool(ChestController _chestToReturn) => chestPool.ReturnItem(_chestToReturn);
        public void Update()
        {
            // Updating All Chests
            ProcessUpdateAllChests();

            // Processing Chests in Queue
            ProcessChestsInQueue();
        }
        private void ProcessUpdateAllChests()
        {
            for (int i = chestPool.pooledItems.Count - 1; i >= 0; i--)
            {
                // Skipping if the pooled item's isUsed is false
                if (!chestPool.pooledItems[i].isUsed)
                {
                    continue;
                }

                var chestController = chestPool.pooledItems[i].Item;
                chestController.Update();

                if (chestController.GetChestModel().ChestState == ChestState.Collected)
                {
                    ReturnChestToPool(chestController);
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
            return chestPool.pooledItems.Any(chest => chest.Item.GetChestModel().ChestState == ChestState.Unlocking
            && chest.isUsed == true);
        }
    }
}