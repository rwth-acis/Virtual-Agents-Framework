using i5.VirtualAgents.BehaviourTrees.Visual;
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
        BOOL,
        LIST_FLOAT,
        TREE
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
    /// Pseudo dictionary, that in contrast to actual dictionaries is serializable, but only offers search in linear time.
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

            throw new KeyNotFoundException(key + " has not been deserialize before.");
        }

        public SerializationEntry<T> Get(int index)
        {
            return data[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Returns true if key was added, returns false when key was already presents and not added again</returns>
        public bool Add(string key, T value)
        {
            if (KeyExists(key))
                return false;
            data.Add(new SerializationEntry<T>(key, value));
            return true;
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
    public class SerializedListFloats : SerializationData<List<float>> { }

    [Serializable]
    public class SerializedTrees : SerializationData<BehaviourTreeAsset> { }

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
        [SerializeField] public SerializedListFloats serializedListFloats = new SerializedListFloats();
        [SerializeField] public SerializedTrees serializedTrees = new SerializedTrees();

        //Saves the order in which the data was serialized. Allows custom inspectors to replicate that order.
        [SerializeField] public List<SerializableType> serializationOrder = new List<SerializableType>();

        #region Overloads for adding data to the serialization
        public void AddSerializedData(string key, Vector3 value)
        {
            if(serializedVectors.Add(key, value))
                serializationOrder.Add(SerializableType.VECTOR3);
        }

        public void AddSerializedData(string key, float value)
        {
            if(serializedFloats.Add(key, value))
                serializationOrder.Add(SerializableType.FLOAT);
        }

        public void AddSerializedData(string key, string value)
        {
            if(serializedStrings.Add(key, value))
                serializationOrder.Add(SerializableType.STRING);
        }

        public void AddSerializedData(string key, int value)
        {
            if(serializedInts.Add(key, value))
                serializationOrder.Add(SerializableType.INT);
        }

        public void AddSerializedData(string key, GameObject value)
        {
            if(serializedGameobjects.Add(key, value))
                serializationOrder.Add(SerializableType.GAMEOBJECT);
        }

        public void AddSerializedData(string key, bool value)
        {
            if(serializedBools.Add(key, value))
                serializationOrder.Add(SerializableType.BOOL);
        }

        public void AddSerializedData(string key, List<float> value)
        {
            if(serializedListFloats.Add(key, value))
                serializationOrder.Add(SerializableType.LIST_FLOAT);
        }

        public void AddSerializedData(string key, BehaviourTreeAsset value)
        {
            if(serializedTrees.Add(key, value))
                serializationOrder.Add(SerializableType.TREE);
        }
        #endregion

        #region Overloads for retrieving serialized data
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

        public List<float> GetSerializedListFloat(string key)
        {
            return serializedListFloats.Get(key);
        }

        public BehaviourTreeAsset GetSerializedTrees(string key)
        {
            return serializedTrees.Get(key);
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
            serializedListFloats.Clear();
            serializedTrees.Clear();
        }

        /// <summary>
        /// Retrieves the key of the item at position index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetKeyByIndex(int index, SerializableType type)
        {
            return type switch
            {
                SerializableType.VECTOR3 => serializedVectors.Get(index).Key,
                SerializableType.FLOAT => serializedFloats.Get(index).Key,
                SerializableType.STRING => serializedStrings.Get(index).Key,
                SerializableType.INT => serializedInts.Get(index).Key,
                SerializableType.GAMEOBJECT => serializedGameobjects.Get(index).Key,
                SerializableType.BOOL => serializedBools.Get(index).Key,
                SerializableType.LIST_FLOAT => serializedListFloats.Get(index).Key,
                SerializableType.TREE => serializedTrees.Get(index).Key,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
