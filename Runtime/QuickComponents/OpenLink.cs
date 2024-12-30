using UnityEngine;

namespace ZeroGame.QuickComponents
{
    [AddComponentMenu("Quick Components/Animator Debugger")]
    public class OpenLink : MonoBehaviour
    {
        public void Open(string link)
        {
            Application.OpenURL(link);
        }
    }
}