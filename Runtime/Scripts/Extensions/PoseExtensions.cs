namespace UnityEngine
{
    public static class PoseExtensions
    {
        public static Pose GetPose(this Transform target) => new Pose(target.position, target.rotation);
        public static void SetPose(this Transform target, Pose pose) => target.SetPositionAndRotation(pose.position, pose.rotation);

        public static Pose GetLocalPose(this Transform target) => new Pose(target.localPosition, target.localRotation);
        public static void SetLocalPose(this Transform target, Pose pose) { target.SetLocalPositionAndRotation(pose.position, pose.rotation); }
    }
}
