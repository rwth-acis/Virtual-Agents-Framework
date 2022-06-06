using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace i5.VirtualAgents
{
    public class TaskSerializer : ScriptableObject, ISerializationCallbackReceiver
    {
        //TODO make private
        //Serialized data
        [SerializeField] public List<Vector3> serializedVectors = new List<Vector3>();
        [SerializeField] public List<string> serializedStrings = new List<string>();
        [SerializeField] public List<float> serializedFloats = new List<float>();
        [SerializeField] public List<int> serializedInts = new List<int>();
        //[SerializeField] private List<string> serializedTypes = new List<string>();

        public Vector3 taskPosition;
        public float speed;
        private ISerializable _serializedTask;
        public ISerializable serializedTask 
        {
            get { return _serializedTask; }
            set 
            {
                _serializedTask = value;
                if (value != null)
                {
                    serializedObjectType = value.GetType().FullName;
                }
            }
        }
        public string serializedObjectType;

        public void PushVector(Vector3 data)
        {
            serializedVectors.Add(data);
        }
        public void PushString(string data)
        {
            serializedStrings.Add(data);
        }
        public void PushFloat(float data)
        {
            serializedFloats.Add(data);
        }
        public void PushInt(int data)
        {
            serializedInts.Add(data);
        }



        public virtual void OnAfterDeserialize()
        {
            if (serializedObjectType != "")
            {
                foreach (var type in TypeCache.GetTypesDerivedFrom<ISerializable>())
                {
                    if (type.FullName == serializedObjectType)
                    {
                        serializedTask = (ISerializable)type.GetConstructor(new Type[0]).Invoke(new object[0]);
                    }
                }
            }
            if (serializedTask != null)
            {
                serializedTask.Deserialize(this);
            }
        }

        public virtual void OnBeforeSerialize()
        {
            if (serializedTask != null)
            {
                serializedTask.Serialize(this);
            }
        }

        //public static ISerializable restoreSerializableInterface() { }
    }
}
