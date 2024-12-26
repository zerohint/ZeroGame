using UnityEngine;

namespace Game.UI
{
    public class OpenLink : MonoBehaviour
    {
        public void Open(string link)
        {
            Application.OpenURL(link);
        }
    }
}