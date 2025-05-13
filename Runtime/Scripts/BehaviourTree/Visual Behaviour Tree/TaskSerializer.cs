using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// 
        /// </summary>
        public void ReSerialize()
        {
            Data.Clear();
            var instance = DeserializeType();
            instance.Serialize(Data);
        }

        // Creates an object from the serialized type
        public ISerializable DeserializeType()
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
        /// Check if Data or, if given, overwriteData still conforms to the serialize method of the task.
        /// I.e. returns false if a key is serialized that isn't present in Data/overwriteData.
         /// </summary>
        /// <param name="overwriteData"></param>
        /// <returns></returns>
        public bool CheckIntegrity(SerializationDataContainer overwriteData = null)
        {
            try
            {
                // Current data
                SerializationDataContainer oldData = overwriteData == null ? Data : overwriteData;
                ISerializable copy = DeserializeType();

                // Data resulting from serializing again
                SerializationDataContainer newData = new SerializationDataContainer();
                List<string> newDataKeys = new List<string>();
                copy.Serialize(newData);

                // Transform serialization order into list of keys
                int wrapperNewData(SerializableType type, int index)
                {
                    newDataKeys.Add(newData.GetKeyByIndex(index,type));
                    return 0;
                }
                List<string> oldDatakeys = new List<string>();
                newData.MapOverData(wrapperNewData);
                int wrapperOldData(SerializableType type, int index)
                {
                    oldDatakeys.Add(oldData.GetKeyByIndex(index,type));
                    return 0;
                }
                oldData.MapOverData(wrapperOldData);

                // If the key lists aren't the same, return false
                if(newDataKeys.Count != oldDatakeys.Count)
                    return false;
                newDataKeys.Sort();
                oldDatakeys.Sort();
                for(int i = 0; i < newDataKeys.Count;i++)
                {
                    if(newDataKeys[i] != oldDatakeys[i])
                        return false;
                }
                return true;
                }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new object from the serialized interface and fills it with the serialized data.
        /// </summary>
        /// <returns></returns>
        public ISerializable GetCopyOfSerializedInterface(SerializationDataContainer overwriteData = null)
        {
            SerializationDataContainer data = overwriteData != null ? overwriteData : Data;
            ISerializable copy = DeserializeType();
            copy.Deserialize(data);
            return copy;
        }
    }
}
