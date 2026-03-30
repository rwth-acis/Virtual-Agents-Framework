using UnityEditor;
using UnityEngine;
namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Manages the changing pathes depending on if the main project is used or if the VAF is included as package.
    /// </summary>
    public static class AssetManager
    {
        private static string _assetPrefix = "";
        /// <summary>
        /// Path prefix to the beviour tree editor assets
        /// </summary>
        public static string assetPrefix
        {
            get
            {
                if (_assetPrefix == "")
                {
                    if (AssetDatabase.IsValidFolder("Packages/com.i5.virtualagents/Editor/UI Builder/Behaviour Tree"))
                    {
                        // We are included as package
                        _assetPrefix = "Packages/com.i5.virtualagents/Editor/UI Builder/Behaviour Tree/";
                    }
                    else
                    {
                        // We are used in the VAF main project
                        _assetPrefix = "Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/";
                    }
                }
                return _assetPrefix;
            }
        }
        /// <summary>
        /// Load asset from the beviour tree asset path
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
