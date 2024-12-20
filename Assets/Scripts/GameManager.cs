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

    [SerializeField] private int metricsCount = 2;
    [SerializeField] private int initialChestCount = 4;
    [SerializeField] private int maxChestCount = 20;

    private int currentChestCount;

    // Start is called before the first frame update
    private void Start()
    {
        // Adding Metrics
        for (int i = 0; i < metricsCount; i++)
        {
            AddMetrics();
        }

        // Adding Chest
        currentChestCount = 0;
        AddGenerateChestButtonToListener();
        for (int i = 0; i < initialChestCount; i++)
        {
            AddChest();
        }
    }
    private void AddMetrics()
    {
        if (metricsPrefab == null)
        {
            Debug.LogWarning("Metrics Prefab reference is null!");
            return;
        }

        GameObject.Instantiate(metricsPrefab, metricsPanel);
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
}
