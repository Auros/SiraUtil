using UnityEngine;

namespace SiraUtil.Objects
{
    /// <summary>
    /// A snapshot of an object.
    /// </summary>
    public struct ObjectState
    {
        /// <summary>
        /// The activity of the object at a point in time.
        /// </summary>
        public readonly bool Active;

        /// <summary>
        /// The position and rotation of the object at a point in time.
        /// </summary>
        public readonly Pose pose;

        /// <summary>
        /// The scale of the object at a point in time.
        /// </summary>
        public readonly Vector3 scale;

        /// <summary>
        /// The tranform of the object.
        /// </summary>
        public readonly Transform transform;

        /// <summary>
        /// The parent of the object at a point in time;
        /// </summary>
        private readonly Transform parent;

        /// <summary>
        /// Initializes a new object state.
        /// </summary>
        /// <param name="transform">The transform of the object</param>
        public ObjectState(Transform transform)
        {
            pose = new Pose(transform.localPosition, transform.localRotation);
            Active = transform.gameObject.activeInHierarchy;
            scale = transform.localScale;
            this.transform = transform;
            parent = transform.parent;

        }

        /// <summary>
        /// If the object still exists, revert its position back to what it was before.
        /// </summary>
        public void Revert()
        {
            if (transform != null)
            {
                transform.SetParent(parent);
                transform.localScale = scale;
                transform.gameObject.SetActive(Active);
                transform.localPosition = pose.position;
                transform.localRotation = pose.rotation;
            }
        }
    }
}