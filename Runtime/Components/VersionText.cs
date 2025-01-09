namespace UnityEngine.UI
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class VersionText : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<TMPro.TMP_Text>().text = $"V {Application.version}";
        }
    }
}
