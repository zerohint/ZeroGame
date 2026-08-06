using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SwipeMenu : MonoBehaviour
{
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Panel[] panels = new Panel[5];

    [SerializeField, Range(0, 4)] private int currentPanel = 2;

    [SerializeField] private float transitionDuration = 0.5f;

    private float screenWidth;

    private void Start()
    {
        screenWidth = mainPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 0; i < panels.Length; i++)
        {
            int index = i; // Capture the correct index in the listener
            panels[i].button.onClick.AddListener(() => SetPanel(index));
        }
        FixWidth();
        SetPanel(panels.Length / 2, true);
    }

    public void SetPanel(int index, bool instant = false)
    {
        if (currentPanel == index) return; // Avoid unnecessary transition

        panels[currentPanel].SetActive(false);
        panels[index].SetActive(true);
        if (instant)
            mainPanel.transform.localPosition = new Vector3(-screenWidth * index, mainPanel.transform.localPosition.y, mainPanel.transform.localPosition.z);
        else
            throw new NotImplementedException("Implement tween");
            // LeanTween.LeanTween.moveX(mainPanel, -screenWidth * index, transitionDuration).setEase(LeanTweenType.easeInOutQuad);

        // Update the current panel index
        currentPanel = index;
    }

    [ContextMenu("Fix Width")]
    private void FixWidth()
    {
        mainPanel.anchoredPosition = new Vector2(-screenWidth * (panels.Length / 2), mainPanel.anchoredPosition.y);
        for (int i = 0; i < panels.Length; i++)
            panels[i].panel.sizeDelta = new Vector2(screenWidth, panels[i].panel.sizeDelta.y);
    }

    [Serializable]
    public class Panel
    {
        public Button button;
        public RectTransform panel;
        public UnityEvent OnOpen;

        private float initialYPosition;

        public void SetActive(bool active)
        {
            var buttonImage = button.GetComponentInChildren<Image>().rectTransform;

            if (initialYPosition == 0f)
            {
                initialYPosition = buttonImage.localPosition.y;
            }

            float targetY = active ? initialYPosition + 25f : initialYPosition;
            throw new NotImplementedException("make tween");
            // LeanTween.LeanTween.moveLocalY(buttonImage.gameObject, targetY, 0.3f).setEase(LeanTweenType.easeInOutQuad);

            OnOpen?.Invoke();
        }
    }


    private void OnValidate()
    {
        SetPanel(currentPanel, true);
    }
}
