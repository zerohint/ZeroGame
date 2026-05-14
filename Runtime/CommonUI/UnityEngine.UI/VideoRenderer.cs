using System.Collections;
using UnityEngine.Video;

namespace UnityEngine.UI
{
    public class VideoRenderer : RawImage
    {
        // NOTE: has editor script
        [SerializeField] private VideoClip videoClip;
        [SerializeField] private Sprite placeholder;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool loop = true;

        public VideoPlayer VideoPlayer { get; protected set; }

        private Texture2D snapshot;

        public void Play(VideoClip video, bool loop = false, VideoRenderMode mode = VideoRenderMode.APIOnly)
        {
            this.texture = TakeSnapshot(this.texture);
            VideoPlayer.renderMode = mode;
            VideoPlayer.clip = video;
            VideoPlayer.isLooping = loop;
            Delayer.DelayWhile(() => !VideoPlayer.isPrepared, () => VideoPlayer.Play());
        }

  

        protected override void Awake()
        {
            base.Awake();
            if (placeholder == null)
                color = new Color(0, 0, 0, 0);
            else
                texture = placeholder.texture;
            if (TryGetComponent<VideoPlayer>(out var vp))
                VideoPlayer = vp;
            else
                VideoPlayer = gameObject.AddComponent<VideoPlayer>();

            VideoPlayer.playOnAwake = playOnAwake;
            VideoPlayer.isLooping = loop;
            VideoPlayer.renderMode = VideoRenderMode.APIOnly;
            VideoPlayer.clip = videoClip;
        }

        protected override void Start()
        {
            base.Start();
            if (videoClip == null)
            {
                return;
            }
            VideoPlayer.Prepare();            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VideoPlayer.prepareCompleted += OnPrepared;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            VideoPlayer.prepareCompleted -= OnPrepared;
        }

        private void OnPrepared(VideoPlayer vp)
        {
            if (color.a == 0)
                color = Color.white;
            if (snapshot.IsExists())
            {
                Destroy(snapshot);
                snapshot = null;
            }
            this.texture = vp.texture;
        }

        /// <summary>
        /// Take a snapshot of the current texture to show while the video is loading.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Texture2D TakeSnapshot(Texture source)
        {
            if (source == null) return null;

            RenderTexture rt = new(source.width, source.height, 0);
            Graphics.Blit(source, rt);
            RenderTexture.active = rt;
            Texture2D tex = new(source.width, source.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            rt.Release();
            snapshot = tex;
            return snapshot;
        }


        protected void OnValidate()
        {

            if (!Application.isPlaying && placeholder != null)
            {
                texture = placeholder.texture;
                SetAllDirty();
            }
            if(VideoPlayer != null)
            {
                VideoPlayer.clip = videoClip;
                VideoPlayer.playOnAwake = playOnAwake;
                VideoPlayer.isLooping = loop;
            }
        }

    }
}
