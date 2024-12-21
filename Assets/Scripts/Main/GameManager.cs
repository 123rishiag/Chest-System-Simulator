using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    [Header("UI Items")]
    [SerializeField] private Transform chestSlotContentPanel;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private Transform metricsPanel;
    [SerializeField] private GameObject metricsPrefab;

    [SerializeField] private Button generateChestButton;

    [Header("Game Items")]
    [SerializeField] private ChestConfig chestConfig;
    [SerializeField] private CurrencyConfig currencyConfig;

    private List<ChestController> chestControllers;
    private List<CurrencyController> currencyControllers;


    private void Awake()
    {
        chestControllers = new List<ChestController>();
        currencyControllers = new List<CurrencyController>();
    }

    private void Start()
    {
        // Adding Chest
        CreateRandomChests();

        // Adding Currencies
        CreateCurrencies();

        // Adding Listeners to Buttons
        AddGenerateChestButtonToListener();
    }

    private void CreateRandomChests()
    {
        for (int i = 0; i < chestConfig.minChestCount; i++)
        {
            AddChest();
        }
    }

    private void AddChest()
    {
        if (chestConfig == null)
        {
            Debug.LogWarning("Chest Scriptable Object reference is null!");
            return;
        }

        if (chestSlotContentPanel == null || chestPrefab == null)
        {
            Debug.LogWarning("Chest Slot Panel or Prefab reference is null!");
            return;
        }

        // Fetching Random Chest
        ChestData chestData = GetRandomChest();
        if(chestData == null)
        {
            Debug.Log("Chest Data is null!");
            return;
        }

        // Initializing a ChestController for random chest
        if (chestControllers.Count < chestConfig.maxChestCount)
        {
            var chestController = new ChestController(chestData, chestSlotContentPanel, chestPrefab);
            chestControllers.Add(chestController);
        }
    }

    private ChestData GetRandomChest()
    {
        if (chestConfig == null || chestConfig.chests.Count == 0)
        {
            Debug.LogWarning("Chest Scriptable Object reference is null or empty!");
            return null;
        }

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

    private void CreateCurrencies()
    {
        if (currencyConfig == null)
        {
            Debug.LogWarning("Currency Scriptable Object reference is null!");
            return;
        }

        if (metricsPanel == null || metricsPrefab == null)
        {
            Debug.LogWarning("Metrics Panel or Prefab reference is null!");
            return;
        }

        // Initializing a CurrencyController for each currency in the config
        foreach (var currencyData in currencyConfig.currencies)
        {
            var currencyController = new CurrencyController(currencyData, metricsPanel, metricsPrefab);
            currencyControllers.Add(currencyController);
        }
    }

    private void AddGenerateChestButtonToListener()
    {
        if (generateChestButton == null)
        {
            Debug.LogWarning("Button reference is null!");
            return;
        }

        generateChestButton.onClick.RemoveAllListeners();
        generateChestButton.onClick.AddListener(AddChest);
    }
}
