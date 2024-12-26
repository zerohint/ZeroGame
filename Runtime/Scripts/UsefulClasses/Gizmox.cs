namespace UnityEngine
{
    /// <summary>
    /// Advanced Gizmos class
    /// </summary>
    public struct Gizmox
    {
        private Vector3 position;

        // Old Gizmos Values
        private Color recentColor;
        private Matrix4x4 recentMatrix;

        private Gizmox(Vector3 position)
        {
            this.position = position;

            recentColor = Gizmos.color;
            recentMatrix = Gizmos.matrix;
        }
        private Gizmox(Vector3 position, Color color) : this(position)
        {
            Gizmos.color = color;
        }

        #region Properties
        public Gizmox SetColor(Color color)
        {
            Gizmos.color = color;
            return this;
        }

        public Gizmox SetRotation(Vector3 rotation)
        {
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), Vector3.one);
            return this;
        }

        /// <summary>
        /// Aside from meaning, restores Gizmos values
        /// </summary>
        public void Draw()
        {
            Gizmos.matrix = recentMatrix;
            Gizmos.color = recentColor;
        }
        #endregion

        #region Shapes
        private Gizmox Rectangle(Vector2 size)
        {
            Vector3 halfSize = size / 2;
            Gizmos.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, 0), position + new Vector3(-halfSize.x, halfSize.y, 0));
            Gizmos.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, 0), position + new Vector3(halfSize.x, halfSize.y, 0));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, halfSize.y, 0), position + new Vector3(halfSize.x, -halfSize.y, 0));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, 0), position + new Vector3(-halfSize.x, -halfSize.y, 0));

            return this;
        }

        private Gizmox Point()
        {
            const float size = 0.3f;
            Gizmos.DrawLine(position + Vector3.up * size, position + Vector3.down * size);
            Gizmos.DrawLine(position + Vector3.right * size, position + Vector3.left * size);
            Gizmos.DrawLine(position + Vector3.forward * size, position + Vector3.back * size);
            return this;
        }

        private Gizmox Cube(Vector3 size)
        {
            Vector3 halfSize = size / 2;
            Gizmos.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
            Gizmos.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, halfSize.y, -halfSize.z));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));

            Gizmos.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), position + new Vector3(-halfSize.x, halfSize.y, halfSize.z));
            Gizmos.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, halfSize.z), position + new Vector3(halfSize.x, halfSize.y, halfSize.z));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, halfSize.y, halfSize.z), position + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, halfSize.z), position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));

            Gizmos.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
            Gizmos.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, halfSize.y, halfSize.z));
            Gizmos.DrawLine(position + new Vector3(halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, halfSize.y, halfSize.z));
            //Gizmos.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, -halfSize))
            return this;
        }
        
        private Gizmox Pose()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Statics
        public static Gizmox Rectangle(Vector3 position, Vector2 size)
        {
            return new Gizmox(position).Rectangle(size);
        }
        public static Gizmox Point(Vector3 position)
        {
            return new Gizmox(position).Point();
        }
        public static Gizmox Cube(Vector3 position, Vector3 size)
        {
            return new Gizmox(position).Cube(size);
        }

        public static void RestoreGizmos()
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
        #endregion
    }
}
