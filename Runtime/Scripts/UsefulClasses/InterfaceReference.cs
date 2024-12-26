namespace UnityEngine
{
    /// <summary>
    /// Interfaces are not serializable, so this is a workaround to allow them to be serialized.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class IRef<T> : ISerializationCallbackReceiver where T : class
    {
        /// <summary>
        /// Serialized interface
        /// </summary>
        public T I { get => Reference as T; }

        /// <summary>
        /// Object reference
        /// </summary>
        public Object Ref => Reference;


        [SerializeField] private Object Reference;


        public static implicit operator bool(IRef<T> ir) => ir.Reference != null;

        public void Set(Object i)
        {
            if(i is not T)
                throw new System.InvalidCastException($"Cannot cast {i.GetType()} to {typeof(T)}");
            Reference = i;
        }

        void OnValidate()
        {
            if (Reference is not T)
            {
                if (Reference is GameObject go)
                {
                    foreach (Component c in go.GetComponents<Component>())
                    {
                        if (c is T)
                        {
                            Reference = c;
                            break;
                        }
                    }
                }
            }
            if (I == null) Reference = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}