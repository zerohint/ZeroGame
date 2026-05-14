using TMPro;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public class VersionText : MonoBehaviour
    {
        [SerializeField] private string prefix = "V ";

        private void Awake()
        {
            GetComponent<TMP_Text>().text = $"{prefix}{Application.version}";
        }
    }
}
