namespace UnityEngine
{
    /// <summary>
    /// All platforms clipboard access.
    /// </summary>
    public static class Clipboard
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void CopyToWebClipboard(string text);
#endif

        public static void CopyToClipboard(string text)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            CopyToWebClipboard(text);
#else
            GUIUtility.systemCopyBuffer = text;
#endif
        }
    }
}
