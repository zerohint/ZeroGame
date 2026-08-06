using UnityEngine;
using TMPro;
using System;

namespace TMPro
{
    // TODO: wrap TMP_InputField
    public class InputFieldHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField; // Inspector'dan bağlayın
        public Action action;
        [SerializeField] private bool aboutCont; // Inspector'dan bağlayın

        void Start()
        {
            // InputField'deki "onEndEdit" olayına metod ekle
            if (aboutCont)
                action += SubmitText;
            inputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        private void OnInputEndEdit(string text)
        {
            // Eğer mobildeyse, "Done" tuşuna basıldığında otomatik tetiklenir.
            // PC'de Enter'a basıldığını manuel kontrol edelim
            if (IsMobilePlatform() || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                action?.Invoke();
        }

        private void SubmitText()
        {
            throw new NotImplementedException();
            // GF.App.User.Local.about = inputField.text;
            // Burada Firebase veya başka bir sistemle kaydetme işlemi yapabilirsin.
        }

        private bool IsMobilePlatform()
        {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }
}
