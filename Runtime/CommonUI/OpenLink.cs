namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenLink : MonoBehaviour
    {
        [SerializeField] private string link;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(link));
        }

        // TODO: onvalidate url check
    }
}