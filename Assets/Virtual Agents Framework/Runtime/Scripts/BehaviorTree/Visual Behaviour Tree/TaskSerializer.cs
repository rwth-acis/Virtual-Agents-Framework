using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// The types that can be serialized using the ISerializable interface.
    /// </summary>
    [Serializable]
    public enum SerializableType
    {
        VECTOR3,
        FLOAT,
        STRING,
        INT
    }

    /// <summary>
    /// Serilaized data identified by a key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SerializationEntry<T>
    {
        public string key;
        public T value;

        public SerializationEntry(string key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// Pseudo dictonary, that in contrast to actual dictonaries is serializable, but only offers search in linear time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SerializationData<T>
    {
        [SerializeField] private List<SerializationEntry<T>> data = new List<SerializationEntry<T>>();

        public T Get(string key)
        {
            foreach (var entry in data)
            {
                if (entry.key == key)
                {
                    return entry.value;
                }
            }
            throw new KeyNotFoundException();
        }

        public SerializationEntry<T> Get(int index)
        {
            return data[index];
        }

        public void Add(string key, T value)
        {
            data.Add(new SerializationEntry<T>(key,value));
        }

        public void Clear()
        {
            data.Clear();
        }
    }

    //Since generic types are not serializable, a new type that derives from the generic version while providing it with a concrete type has to be created. 
    [Serializable]
    public class SerializedVectors : SerializationData<Vector3> { }
    [Serializable]
    public class SerializedFloats : SerializationData<float> { }
    [Serializable]
    public class SerializedStrings : SerializationData<string> { }
    [Serializable]
    public class SerializedInts : SerializationData<int> { }

    /// <summary>
    /// Allows to serialize tasks that implement the ISerializable interface.
    /// </summary>
    public class TaskSerializer : ScriptableObject
    {
        //Serialized data
        [SerializeField] private SerializedVectors serializedVectors = new SerializedVectors();
        [SerializeField] private SerializedFloats serializedFloats = new SerializedFloats();
        [SerializeField] private SerializedStrings serializedStrings = new SerializedStrings();
        [SerializeField] private SerializedInts serializedInts = new SerializedInts();

        //Saves the order in which the data was serialized. Allows coustom inspectors to replicate that order.
        [SerializeField] public List<SerializableType> serializationOrder = new List<SerializableType>();

        //The name of the type that was serialized
        [SerializeField] private string serializedObjectType;

        public void SetSerializedType(ISerializable serializedTask)
        {
            //Save the name of the type
            serializedObjectType = serializedTask.GetType().FullName;

            //Serialize it once in order to retrive the default values of this task
            ClearSerializedData();
            serializedTask.Serialize(this);
        }

        #region Overloads for adding data to the serialization
        public void AddSerializedData(string key, Vector3 value)
        {
            serializationOrder.Add(SerializableType.VECTOR3);
            serializedVectors.Add(key, value);
        }

        public void AddSerializedData(string key, float value)
        {
            serializationOrder.Add(SerializableType.FLOAT);
            serializedFloats.Add(key, value);
        }

        public void AddSerializedData(string key, string value)
        {
            serializationOrder.Add(SerializableType.STRING);
            serializedStrings.Add(key, value);
        }

        public void AddSerializedData(string key, int value)
        {
            serializationOrder.Add(SerializableType.INT);
            serializedInts.Add(key, value);
        }
        #endregion

        #region Overloads for retriving serialized data
        public Vector3 GetSerializedVector(string key)
        {
            return serializedVectors.Get(key);
        }

        public float GetSerializedFloat(string key)
        {
            return serializedFloats.Get(key);
        }

        public string GetSerializedString(string key)
        {
            return serializedStrings.Get(key);
        }

        public int GetSerializedInt(string key)
        {
            return serializedInts.Get(key);
        }
        #endregion


        

        /// <summary>
        /// Retrives the key of the item at position index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetKeyByIndex(int index, SerializableType type)
        {
            switch (type)
            {
                case SerializableType.VECTOR3:
                    return serializedVectors.Get(index).key;
                case SerializableType.FLOAT:
                    return serializedFloats.Get(index).key;
                default:
                    return "";
            }
        }

        //Creates an object from the serialized type
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
            copy.Deserialize(this);
            return copy;
        }

        // Deletes everything that was serialized
        private void ClearSerializedData()
        {
            serializationOrder.Clear();
            serializedVectors.Clear();
            serializedStrings.Clear();
            serializedFloats.Clear();
            serializedInts.Clear();
        }
    }
}
