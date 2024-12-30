using UnityEngine;
using UnityEngine.Events;

namespace ZeroGame.QuickComponents
{
    [AddComponentMenu("Quick Components/Keyboard Event")]
    public class KeyboardEvent : MonoBehaviour
    {
        [SerializeField] private KeyCode KeyboardButton = KeyCode.S;
        [SerializeField] private UnityEvent unityEvent;
        

        private void Update()
        {
            if (Input.GetKeyDown(KeyboardButton))
            {
                unityEvent?.Invoke();
            }
        }
    }
}