using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace i5.VirtualAgents.Editor
{
    public static class AssetManager
    {
        private static string _assetPrefix = "";
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
                        // We are used in the agent main project
                        _assetPrefix = "Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/";
                    }
                }
                return _assetPrefix;
            }
        }
            public static T Load<T>(string suffix) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(assetPrefix + suffix);
        }
    }
}
