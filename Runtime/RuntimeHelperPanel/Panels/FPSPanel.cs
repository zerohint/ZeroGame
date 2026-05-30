using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace ZeroGame.RHP
{
    public class FPSPanel : RHPPanelBase
    {
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private Button smoothToggleButton;
        [SerializeField] private TMP_Text smoothToggleText;
        [Header("Target Frame Buttons")]
        [SerializeField] private Button targetFrameInfButton;
        [SerializeField] private Button targetFrame60Button;
        [SerializeField] private Button targetFrame30Button;

        private float FPS => 1.0f / Time.deltaTime;
        private bool smoothView = false;
        private float fps;

        private void Start()
        {
            smoothToggleButton.onClick.AddListener(SmoothViewToggle);
            targetFrameInfButton.onClick.AddListener(() => { SetFrame(999); });
            targetFrame60Button.onClick.AddListener(() => { SetFrame(60); });
            targetFrame30Button.onClick.AddListener(() => { SetFrame(30); });
        }

        private void Update()
        {
            fps = smoothView ? Mathf.Lerp(fps, FPS, Time.deltaTime) : FPS;
            fpsText.text = "FPS: " + fps.ToString("F1");
        }

        private void SetFrame(int frame)
        {
            Application.targetFrameRate = frame;
            StartCoroutine(DirectUpdateFps());
        }

        private void SmoothViewToggle()
        {
            smoothView = !smoothView;
            smoothToggleText.text = smoothView ? "-" : "~";
            StartCoroutine(DirectUpdateFps());
        }

        private IEnumerator DirectUpdateFps()
        {
            yield return null;
            fps = FPS;
        }
    }
}
