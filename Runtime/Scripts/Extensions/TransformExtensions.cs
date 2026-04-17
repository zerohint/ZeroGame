namespace UnityEngine
{
    public static class TransformExtensions
    {
        public static Pose GetPose(this Transform t) => new(t.position, t.rotation);
        public static void SetPose(this Transform t, Pose pose) => t.SetPositionAndRotation(pose.position, pose.rotation);

        public static Pose GetLocalPose(this Transform t) => new(t.localPosition, t.localRotation);
        public static void SetLocalPose(this Transform t, Pose pose) { t.SetLocalPositionAndRotation(pose.position, pose.rotation); }


        /// <summary>
        /// Destroy all children
        /// </summary>
        /// <param name="t"></param>
        public static void ClearChildren(this Transform t)
        {
            if (t == null) return;
            if (t.childCount == 0) return;
            foreach (Transform child in t)
                GameObject.Destroy(child.gameObject);
        }
    }
}
