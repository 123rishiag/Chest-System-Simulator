using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class GameManager : MonoBehaviour
{
    [Header("UI Items")]
    [SerializeField] private Transform chestSlotContentPanel;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private Transform metricsPanel;
    [SerializeField] private GameObject metricsPrefab;

    [SerializeField] private GameObject chestProcessingPanel;
    [SerializeField] private Button chestProcessingActionOneButton;
    [SerializeField] private Button chestProcessingActionTwoButton;

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
        // Adding Currencies
        CreateCurrencies();

        // Adding Chest
        CreateRandomChests();

        // Adding Listeners to Buttons
        AddGenerateChestButtonToListener();
    }

    private void Update()
    {
        // Updating Chests
        for (int i = chestControllers.Count - 1; i >= 0; i--)
        {
            var chestController = chestControllers[i];
            chestController.Update();

            if (chestController.ChestState == ChestState.Collected)
            {
                StartCoroutine(RemoveChest(chestController, 2f));
            }
        }
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
    public int GetCurrency(CurrencyType _currencyType)
    {
        var currencyController = GetCurrencyController(_currencyType);
        return currencyController.CurrencyValue;
    }
    public CurrencyData GetCurrencyData(CurrencyType _currencyType)
    {
        var currencyController = GetCurrencyController(_currencyType);
        return currencyController.CurrencyData;
    }
    public void AddCurrency(CurrencyType _currencyType, int _value)
    {
        var currencyController = GetCurrencyController(_currencyType);
        currencyController.AddCurrency(_value);
    }
    public void DeductCurrency(CurrencyType _currencyType, int _value)
    {
        var currencyController = GetCurrencyController(_currencyType);
        currencyController.DeductCurrency(_value);
    }
    private CurrencyController GetCurrencyController(CurrencyType _currencyType)
    {
        foreach (var currencyController in currencyControllers)
        {
            if (currencyController.CurrencyType == _currencyType)
            {
                return currencyController;
            }
        }
        return null;
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
            var chestController = new ChestController(this, chestData, chestSlotContentPanel, chestPrefab);
            chestControllers.Add(chestController);
        }
    }
    private IEnumerator RemoveChest(ChestController _chestController, float _timeInSeconds)
    {
        yield return new WaitForSeconds(_timeInSeconds);
        _chestController.Destroy();
        chestControllers.Remove(_chestController);
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

    public void ConfigureButtons(System.Action _onButton1Click, System.Action _onButton2Click, string _button1Text, string _button2Text)
    {
        TMP_Text chestProcessingActionOneText  = chestProcessingActionOneButton.GetComponentInChildren<TMP_Text>();
        TMP_Text chestProcessingActionTwoText = chestProcessingActionTwoButton.GetComponentInChildren<TMP_Text>();

        if (chestProcessingPanel == null)
        {
            Debug.LogError("Chest Processing Panel not found!");
            return;
        }
        if (chestProcessingActionOneText == null)
        {
            Debug.LogError("Chest Processing Action One Text Field not found in panel!!");
            return;
        }
        if (chestProcessingActionTwoText == null)
        {
            Debug.LogError("Chest Processing Action Two Text Field not found in panel!!");
            return;
        }

        // Set the panel active
        chestProcessingPanel.SetActive(true);

        // Configure Button 1
        chestProcessingActionOneText.text = _button1Text;
        chestProcessingActionOneButton.onClick.RemoveAllListeners();
        chestProcessingActionOneButton.onClick.AddListener(() => {
            _onButton1Click?.Invoke();
            chestProcessingPanel.SetActive(false);
        });

        // Configure Button 2
        chestProcessingActionTwoText.text = _button2Text;
        chestProcessingActionTwoButton.onClick.RemoveAllListeners();
        chestProcessingActionTwoButton.onClick.AddListener(() => {
            _onButton2Click?.Invoke();
            chestProcessingPanel.SetActive(false);
        });
    }
}
