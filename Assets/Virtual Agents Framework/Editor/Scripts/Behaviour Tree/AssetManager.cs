using UnityEditor;
using UnityEngine;
namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Manages the changing paths depending on if the main project is used or if the VAF is included as package.
    /// </summary>
    public static class AssetManager
    {
        private static string _assetPrefix = "";
        /// <summary>
        /// Path prefix to the behaviour tree editor assets
        /// </summary>
        public static string assetPrefix
        {
            get
            {
                if (_assetPrefix != "") return _assetPrefix;
                // We are included as package
                _assetPrefix = AssetDatabase.IsValidFolder("Packages/com.i5.virtualagents/Editor/UI Builder/Behaviour Tree") ? "Packages/com.i5.virtualagents/Editor/UI Builder/Behaviour Tree/" :
                    // We are used in the VAF main project
                    "Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/";
                return _assetPrefix;
            }
        }
        /// <summary>
        /// Load asset from the behaviour tree asset path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetSuffix"></param>
        /// <returns></returns>
        public static T Load<T>(string assetSuffix) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(assetPrefix + assetSuffix);
        }
    }
}
