namespace Redellion.API
{
    public abstract class RequestBase
    {
        /// <summary>
        /// Api base name
        /// </summary>
        internal abstract string ApiName { get; }

        /// <summary>
        /// Api request name
        /// </summary>
        internal abstract string RequestName { get; }

        /// <summary>
        /// Object to be posted by converting to json
        /// </summary>
        internal abstract object PostData { get; }

        internal string appId => "#";//Kit.RedellionConfigSC.Instance.apiSettings.appId;
    }

}