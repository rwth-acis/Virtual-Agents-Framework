using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace InternalTools
{
    public class PackageExporter: EditorWindow
    {
        [MenuItem("Virtual Agents Framework/Rename sample to sample~")]
        static void RenameForRelease()
        {
            string path = "Assets/Virtual Agents Framework";
            Debug.Log(AssetDatabase.RenameAsset(path + "/Samples", "Samples~"));
        }
        [MenuItem("Virtual Agents Framework/Rename sample~ to sample")]
        static void RenameForDevelopment()
        {
            
            string path = "Assets/Virtual Agents Framework";
            Directory.Move(path + "/Samples~", path + "/Samples");
            AssetDatabase.Refresh();
        }

    }
}