using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
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
        INT,
        GAMEOBJECT,
        BOOL
    }

    /// <summary>
    /// Serialized data identified by a key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SerializationEntry<T>
    {
        public string Key;
        public T Value;

        public SerializationEntry(string key, T value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    /// <summary>
    /// Pseudo dictonary, that in contrast to actual dictonaries is serializable, but only offers search in linear time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SerializationData<T>
    {
        [SerializeField] public List<SerializationEntry<T>> data = new List<SerializationEntry<T>>();

        public T Get(string key)
        {
            foreach (var entry in data)
            {
                if (entry.Key == key)
                {
                    return entry.Value;
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
            data.Add(new SerializationEntry<T>(key, value));
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool KeyExists(string key)
        {
            foreach (var entry in data)
            {
                if (entry.Key == key)
                {
                    return true;
                }
            }

            return false;
        }
    }

    // Since generic types are not serializable, a new type that derives from the generic version while providing it with a concrete type has to be created. 
    [Serializable]
    public class SerializedVectors : SerializationData<Vector3> { }
    [Serializable]
    public class SerializedFloats : SerializationData<float> { }
    [Serializable]
    public class SerializedStrings : SerializationData<string> { }
    [Serializable]
    public class SerializedInts : SerializationData<int> { }
    [Serializable]
    public class SerializedGameobjects : SerializationData<GameObject> { }
    [Serializable]
    public class SerializedBools : SerializationData<bool> { }

    [Serializable]
    public class SerializationDataContainer
    {
        //Serialized data
        [SerializeField] public SerializedVectors serializedVectors = new SerializedVectors();
        [SerializeField] public SerializedFloats serializedFloats = new SerializedFloats();
        [SerializeField] public SerializedStrings serializedStrings = new SerializedStrings();
        [SerializeField] public SerializedInts serializedInts = new SerializedInts();
        [SerializeField] public SerializedGameobjects serializedGameobjects = new SerializedGameobjects();
        [SerializeField] public SerializedBools serializedBools = new SerializedBools();

        //Saves the order in which the data was serialized. Allows coustom inspectors to replicate that order.
        [SerializeField] public List<SerializableType> serializationOrder = new List<SerializableType>();

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

        public void AddSerializedData(string key, GameObject value)
        {
            serializationOrder.Add(SerializableType.GAMEOBJECT);
            serializedGameobjects.Add(key,value);
        }

        public void AddSerializedData(string key, bool value)
        {
            serializationOrder.Add(SerializableType.BOOL);
            serializedBools.Add(key, value);
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

        public GameObject GetSerializedGameobjects(string key)
        {
            return serializedGameobjects.Get(key);
        }

        public bool GetSerializedBool(string key)
        {
            return serializedBools.Get(key);
        }

        #endregion

        /// <summary>
        /// Deletes everything that was serialized
        /// </summary>
        public void Clear()
        {
            serializationOrder.Clear();
            serializedVectors.Clear();
            serializedStrings.Clear();
            serializedFloats.Clear();
            serializedInts.Clear();
            serializedGameobjects.Clear();
            serializedBools.Clear();
        }

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
                    return serializedVectors.Get(index).Key;
                case SerializableType.FLOAT:
                    return serializedFloats.Get(index).Key;
                case SerializableType.STRING:
                    return serializedStrings.Get(index).Key;
                case SerializableType.INT:
                    return serializedInts.Get(index).Key;
                case SerializableType.GAMEOBJECT:
                    return serializedGameobjects.Get(index).Key;
                case SerializableType.BOOL:
                    return serializedBools.Get(index).Key;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
