using UnityEditor;
using UnityEngine;

namespace ZeroGame.QuickComponents
{
    [AddComponentMenu("QuickComponents/Screen Shooter")]
    public class ScreenShooter : MonoBehaviour
    {
        [SerializeField] private KeyCode ScreenshotButton = KeyCode.S;

        private static int CaptureCount
        {
            get { return EditorPrefs.GetInt("ScreenshotCaptureStep", 0); }
            set { EditorPrefs.SetInt("ScreenshotCaptureStep", value); }
        }

        private void Update()
        {
            if (Input.GetKeyDown(ScreenshotButton))
            {
                ScreenCapture.CaptureScreenshot("Recordings/Capture-" + CaptureCount + ".png");
                CaptureCount++;
                Debug.Log("Screen Captured");
            }
        }
    }
}
