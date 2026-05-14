using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class ButtonToggle : MonoBehaviour
    {
        public UnityEvent<int> onValueChange;
        public int Value { get; private set; } = -1;

        [SerializeField] private Button[] optionButtons;

        private void Awake()
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                optionButtons[index].onClick.AddListener(() => Select(index));
            }
        }

        private void Start()
        {
            if (Value == -1)
                Select(0);
        }


        public void Select(int option)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                var img = optionButtons[i].image;
                Debug.Assert(img != null);
                Color color = img.color;
                color.a = i == option ? 1 : 0;
                img.color = color;
            }
            Value = option;
        }
    }
}
