using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
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

            //Serialize it once in order to retrieve the default values of this task
            Data.Clear();
            serializedTask.Serialize(Data);
        }

        // Creates an object from the serialized type
        private ISerializable DeserializeType()
        {
#if UNITY_EDITOR
            //More efficient way to get the type in the editor
            foreach (var type in TypeCache.GetTypesDerivedFrom<ISerializable>())
            {
                if (type.FullName == serializedObjectType)
                {
                    return (ISerializable)type.GetConstructor(new Type[0]).Invoke(new object[0]);
                }
            }
#endif


#if !UNITY_EDITOR
            var serializableType = typeof(ISerializable);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (serializableType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        if (type.FullName == serializedObjectType)
                        {
                            return (ISerializable)Activator.CreateInstance(type);
                        }
                    }
                }
            }
#endif
            throw new Exception("Type " + serializedObjectType + " could not be found. It is likely that the name of a task in the tree was changed. Please update the name accordingly in the asset file of that tree.");
        }

        /// <summary>
        /// Creates a new object from the serialized interface and fills it with the serialized data.
        /// </summary>
        /// <returns></returns>
        public ISerializable GetCopyOfSerializedInterface(SerializationDataContainer overwriteData = null)
        {
            ISerializable copy = DeserializeType();
            try
            {
                copy.Deserialize(overwriteData != null ? overwriteData : Data);
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogWarning("One node seems to have added new attributes since the tree got saved last. Node will now be updated. <b> Please save the tree now and replace the tree used in the scene. </b> Error: " + e + " ");
                copy.Serialize(overwriteData != null ? overwriteData : Data);
                copy.Deserialize(overwriteData != null ? overwriteData : Data);
            }

            return copy;
        }
    }
}
