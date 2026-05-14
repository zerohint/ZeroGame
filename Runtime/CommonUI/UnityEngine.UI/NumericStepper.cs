using TMPro;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class NumericStepper : MonoBehaviour
    {
        public UnityEvent<int> onValueChange;
        public int Value { get; private set; } = 0;

        [SerializeField] private int beginValue;
        [SerializeField] private int minValue;
        [SerializeField] private int maxValue;
        [SerializeField] private int step;
        [Header("References")]
        [SerializeField] private Button positiveButton;
        [SerializeField] private Button negativeButton;
        [SerializeField] private TMP_Text valueText;

        private void Start()
        {
            positiveButton.onClick.AddListener(OnPositivePress);
            negativeButton.onClick.AddListener(OnNegativePress);
            if (Value == -1)
                Set(beginValue);
        }

        public void Set(int value)
        {
            Debug.Log("set");
            Value = Mathf.Clamp(value, minValue, maxValue);
            valueText.text = Value.ToString();
            onValueChange?.Invoke(Value);
        }

        private void OnPositivePress() => Set(Value + step);

        private void OnNegativePress() => Set(Value - step);
    }
}
