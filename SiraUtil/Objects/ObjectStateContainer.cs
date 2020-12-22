using System;
using UnityEngine;
using System.Collections.Generic;

namespace SiraUtil.Objects
{
    /// <summary>
    /// A container for object states.
    /// </summary>
    public struct ObjectStateContainer
    {
        /// <summary>
        /// The objects in this container.
        /// </summary>
        public readonly ObjectState[] objects;

        /// <summary>
        /// Initializes a new object state container.
        /// </summary>
        /// <param name="mainParent">The object to snapshot.</param>
        public ObjectStateContainer(GameObject mainParent)
        {
            if (mainParent == null)
            {
                throw new ArgumentNullException();
            }
            var populator = new List<ObjectState>();
            Snapshot(mainParent.transform, ref populator);
            objects = populator.ToArray();
        }

        private static void Snapshot(Transform transform, ref List<ObjectState> populator)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                populator.Add(new ObjectState(child));
                Snapshot(child, ref populator);
            }
        } 

        /// <summary>
        /// Reverts all the objects into the container to their original position.
        /// </summary>
        public void Revert()
        {
            foreach (var obj in objects)
            {
                obj.Revert();
            }
        }
    }
}