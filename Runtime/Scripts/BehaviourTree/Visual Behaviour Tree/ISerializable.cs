namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Allows a task be be serialized. Necessary in order to use them with the visual Behaviour Tree Editor.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serializes the task to be used in the Behaviour Tree Editor
        /// </summary>
        void Serialize(SerializationDataContainer serializer);

        /// <summary>
        /// Deserializes the task to be used in the Behaviour Tree Editor
        /// </summary>
        void Deserialize(SerializationDataContainer serializer);
    }
}
