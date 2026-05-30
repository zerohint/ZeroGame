using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ZeroGame.RHP
{
    internal class DoubleTapDetect : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnDoubleClick;


        private const float DOUBLE_CLICK_TIME = 0.3f;

        private float lastClickTime = 0;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - lastClickTime < DOUBLE_CLICK_TIME)
            {
                OnDoubleClick?.Invoke();
            }
            lastClickTime = Time.time;
        }
    }
}
