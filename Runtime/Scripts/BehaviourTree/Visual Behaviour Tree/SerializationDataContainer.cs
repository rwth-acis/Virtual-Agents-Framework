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
        public void SetValue(string key, T value)
        {
            foreach (var entry in data)
            {
                if (entry.Key == key)
                {
                    entry.Value = value;
                }
            }
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
    public class SerializedGameObjects : SerializationData<GameObject> { }
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
        [SerializeField] public SerializedGameObjects serializedGameobjects = new SerializedGameObjects();
        [SerializeField] public SerializedBools serializedBools = new SerializedBools();
        [SerializeField] public SerializedListFloats serializedListFloats = new SerializedListFloats();
        [SerializeField] public SerializedTrees serializedTrees = new SerializedTrees();

        //Saves the order in which the data was serialized. Allows custom inspectors to replicate that order.
        [SerializeField] public List<SerializableType> serializationOrder = new List<SerializableType>();

        #region Overloads for adding data to the serialization
        public void AddSerializedData(string key, Vector3 value)
        {
            if (serializedVectors.Add(key, value))
                serializationOrder.Add(SerializableType.VECTOR3);
        }

        public void AddSerializedData(string key, float value)
        {
            if (serializedFloats.Add(key, value))
                serializationOrder.Add(SerializableType.FLOAT);
        }

        public void AddSerializedData(string key, string value)
        {
            if (serializedStrings.Add(key, value))
                serializationOrder.Add(SerializableType.STRING);
        }

        public void AddSerializedData(string key, int value)
        {
            if (serializedInts.Add(key, value))
                serializationOrder.Add(SerializableType.INT);
        }

        public void AddSerializedData(string key, GameObject value)
        {
            if (serializedGameobjects.Add(key, value))
                serializationOrder.Add(SerializableType.GAMEOBJECT);
        }

        public void AddSerializedData(string key, bool value)
        {
            if (serializedBools.Add(key, value))
                serializationOrder.Add(SerializableType.BOOL);
        }

        public void AddSerializedData(string key, List<float> value)
        {
            if (serializedListFloats.Add(key, value))
                serializationOrder.Add(SerializableType.LIST_FLOAT);
        }

        public void AddSerializedData(string key, BehaviourTreeAsset value)
        {
            if (serializedTrees.Add(key, value))
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

        /// <summary>
        /// Can be used to rename a key or to automatically update a key in an old file. Value of the old key is copied to the new key.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns>Returns true, if successfully renamed otherwise false</returns>
        public bool Replace(String oldName, String newName)
        {
            if (oldName == newName)
                return true;
            if (serializedVectors.KeyExists(oldName))
            {
                // Add the new key and copy the value
                // If the new key already exists, the value changed to that of the old key
                if (!serializedVectors.Add(newName, serializedVectors.Get(oldName)))
                    serializedVectors.SetValue(newName, serializedVectors.Get(oldName));
                serializedVectors.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.VECTOR3);
            }
            else if (serializedStrings.KeyExists(oldName))
            {
                if (!serializedStrings.Add(newName, serializedStrings.Get(oldName)))
                    serializedStrings.SetValue(newName, serializedStrings.Get(oldName));
                serializedStrings.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.STRING);
            }
            else if (serializedInts.KeyExists(oldName))
            {
                if (!serializedInts.Add(newName, serializedInts.Get(oldName)))
                    serializedInts.SetValue(newName, serializedInts.Get(oldName));
                serializedInts.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.INT);
            }
            else if (serializedFloats.KeyExists(oldName))
            {
                if (!serializedFloats.Add(newName, serializedFloats.Get(oldName)))
                    serializedFloats.SetValue(newName, serializedFloats.Get(oldName));
                serializedFloats.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.FLOAT);
            }
            else if (serializedGameobjects.KeyExists(oldName))
            {
                if (!serializedGameobjects.Add(newName, serializedGameobjects.Get(oldName)))
                    serializedGameobjects.SetValue(newName, serializedGameobjects.Get(oldName));
                serializedGameobjects.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.GAMEOBJECT);
            }
            else if (serializedBools.KeyExists(oldName))
            {
                if (!serializedBools.Add(newName, serializedBools.Get(oldName)))
                    serializedBools.SetValue(newName, serializedBools.Get(oldName));
                serializedBools.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.BOOL);
            }
            else if (serializedListFloats.KeyExists(oldName))
            {
                if (!serializedListFloats.Add(newName, serializedListFloats.Get(oldName)))
                    serializedListFloats.SetValue(newName, serializedListFloats.Get(oldName));
                serializedListFloats.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.LIST_FLOAT);
            }
            else if (serializedTrees.KeyExists(oldName))
            {
                if (!serializedTrees.Add(newName, serializedTrees.Get(oldName)))
                    serializedTrees.SetValue(newName, serializedTrees.Get(oldName));
                serializedTrees.data.RemoveAll(x => x.Key == oldName);
                RemoveUnnecessaryEntriesInOrderOfType(SerializableType.TREE);
            }
            else
            {
                return false;
            }
            return true;
        }

        private void RemoveUnnecessaryEntriesInOrderOfType(SerializableType type)
        {
            int amountOfEntries = type switch
            {
                SerializableType.VECTOR3 => serializedVectors.data.Count,
                SerializableType.FLOAT => serializedFloats.data.Count,
                SerializableType.STRING => serializedStrings.data.Count,
                SerializableType.INT => serializedInts.data.Count,
                SerializableType.GAMEOBJECT => serializedGameobjects.data.Count,
                SerializableType.BOOL => serializedBools.data.Count,
                SerializableType.LIST_FLOAT => serializedListFloats.data.Count,
                SerializableType.TREE => serializedTrees.data.Count,
                _ => throw new NotImplementedException(),
            };
            // Remove all entries of the type that are not needed anymore
            // This can change the order of the entries but only happens when a tree is updated
            int i = 0;
            List<SerializableType> newSerializationOrder = new List<SerializableType>();
            foreach (SerializableType orderEntry in serializationOrder)
            {
                if (orderEntry == type)
                {
                    i++;
                    if (i <= amountOfEntries)
                    {
                        newSerializationOrder.Add(orderEntry);
                    }
                }
                else
                {
                    newSerializationOrder.Add(orderEntry);
                }
            }
            this.serializationOrder = newSerializationOrder;
        }
    }
}
