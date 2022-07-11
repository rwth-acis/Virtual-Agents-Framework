using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.BehaviourTrees;

namespace i5.VirtualAgents.TaskSystem
{


    /// <summary>
    /// Allows to serialize tasks that implement the ISerializable interface.
    /// </summary>
    public class TaskSerializer : ScriptableObject
    {
        [SerializeField] public SerializationDataContainer Data = new SerializationDataContainer();

        //The name of the type that was serialized
        [SerializeField] private string serializedObjectType;

        public void SetSerializedType(ISerializable serializedTask)
        {
            //Save the name of the type
            serializedObjectType = serializedTask.GetType().FullName;

            //Serialize it once in order to retrive the default values of this task
            Data.Clear();
            serializedTask.Serialize(Data);
        }

        // Creates an object from the serialized type
        private ISerializable DeserializeType()
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom<ISerializable>())
            {
                if (type.FullName == serializedObjectType)
                {
                    return (ISerializable)type.GetConstructor(new Type[0]).Invoke(new object[0]);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new object from the serialized interface and fills it with the serialized data.
        /// </summary>
        /// <returns></returns>
        public ISerializable GetCopyOfSerializedInterface()
        {
            ISerializable copy = DeserializeType();
            copy.Deserialize(Data);
            return copy;
        }
    }
}
