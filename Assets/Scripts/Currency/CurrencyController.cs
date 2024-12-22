using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyController
{
    private TMP_Text currencyText;

    public CurrencyController(CurrencyData _currencyData, Transform _parentTransform, GameObject _prefab)
    {
        // Initializing variables
        CurrencyData = _currencyData;
        CurrencyType = _currencyData.currencyType;
        CurrencyValue = _currencyData.initialValue;

        // Instantiating the prefab under the parent transform
        GameObject instance = Object.Instantiate(_prefab, _parentTransform);

        if (instance == null)
        {
            Debug.LogError("Currency Prefab not found!!");
            return;
        }

        // Finding Currency UI Elements
        currencyText = instance.transform.Find("MetricsText").GetComponent<TMP_Text>();
        Image imageField = instance.transform.Find("MetricsImage").GetComponent<Image>();

        if (currencyText == null)
        {
            Debug.LogError("Currency Text Field not found in prefab!!");
            return;
        }
        else
        {
            currencyText.text = CurrencyValue.ToString();
        }

        if (imageField == null)
        {
            Debug.LogError("Currency Image Field not found in prefab!!");
        }
        else
        {
            imageField.sprite = _currencyData.currencyImage;
            imageField.color = new Color(_currencyData.imageColor.r, _currencyData.imageColor.g, _currencyData.imageColor.b, 1.0f);
        }
    }

    // Getters
    public CurrencyData CurrencyData { get; private set; }
    public CurrencyType CurrencyType { get; private set; }
    public int CurrencyValue { get; private set; }

    // Setters
    public void AddCurrency(int _value)
    {
        CurrencyValue += _value;
        currencyText.text = CurrencyValue.ToString();
    }
    public void DeductCurrency(int _value)
    {
        CurrencyValue -= _value;
        if (CurrencyValue < 0)
        {
            CurrencyValue = 0;
        }
        currencyText.text = CurrencyValue.ToString();
    }
}