using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    [Header("UI Items")]
    [SerializeField] private Transform metricsPanel;
    [SerializeField] private GameObject metricsPrefab;
    [SerializeField] private Transform chestSlotContentPanel;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private Button generateChestButton;

    [SerializeField] private int initialChestCount = 4;
    [SerializeField] private int maxChestCount = 20;

    private int currentChestCount;

    [Header("Game Items")]
    [SerializeField] private CurrencyConfig currencyConfig;

    private List<CurrencyController> currencyControllers;


    private void Awake()
    {
        currencyControllers = new List<CurrencyController>();
    }

    private void Start()
    {
        // Adding Currencies
        CreateCurrencies();

        // Adding Chest
        currentChestCount = 0;
        AddGenerateChestButtonToListener();
        for (int i = 0; i < initialChestCount; i++)
        {
            AddChest();
        }
    }

    private void AddChest()
    {
        if (chestPrefab == null)
        {
            Debug.LogWarning("Chest Prefab reference is null!");
            return;
        }

        if (currentChestCount < maxChestCount)
        {
            GameObject.Instantiate(chestPrefab, chestSlotContentPanel);
            currentChestCount++;
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
}
