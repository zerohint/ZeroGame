namespace Redellion.API
{
    public class CheckForUpdates : RequestBase
    {
        internal override string ApiName => "CheckForUpdates";

        internal override string RequestName => "Check";

        internal override object PostData => new { appId, platform };

        public string platform =
#if UNITY_EDITOR
                "editor";
#elif UNITY_WEBGL
                "webgl";
#elif UNITY_ANDROID
                "android";
#elif UNITY_IOS
                "ios";
#elif UNITY_STANDALONE_WIN
                "windows";
#elif UNITY_STANDALONE_LINUX
                "linux";
#elif UNITY_STANDALONE_OSX
                "osx";
#else
                "unknown";
#endif
    }
}
