using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ServiceLocator.Currency
{
    public class CurrencyView : MonoBehaviour
    {
        // UI Elements
        [Header("UI Elements")]
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private Image currencyImage;

        // Private Variables
        private CurrencyController currencyController;

        private void ValidateReferences()
        {
            if (currencyText == null)
            {
                Debug.LogError("Currency Text Field not found in prefab!!");
                return;
            }

            if (currencyImage == null)
            {
                Debug.LogError("Currency Image Field not found in prefab!!");
                return;
            }
        }

        // Setters
        public void SetViewProperties(CurrencyController _currencyController)
        {
            // Setting Variables
            currencyController = _currencyController;

            // Validating References
            ValidateReferences();
        }
        public void UpdateUI()
        {
            currencyText.text = currencyController.GetCurrencyModel().CurrencyValue.ToString();
            currencyImage.sprite = currencyController.GetCurrencyModel().CurrencyData.currencyImage;
            currencyImage.color = new Color(currencyController.GetCurrencyModel().CurrencyData.imageColor.r,
                currencyController.GetCurrencyModel().CurrencyData.imageColor.g,
                currencyController.GetCurrencyModel().CurrencyData.imageColor.b,
                1.0f);
        }
    }
}