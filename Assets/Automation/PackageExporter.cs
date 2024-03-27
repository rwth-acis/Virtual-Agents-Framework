using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace InternalTools
{
    public class PackageExporter: EditorWindow
    {
        static string path = "Assets/Virtual Agents Framework";
        [MenuItem("Virtual Agents Framework/Rename sample to sample~")]
        static void RenameForRelease()
        {
            Directory.Move(path + "/Samples", path + "/Samples~");
            AssetDatabase.Refresh();
        }
        [MenuItem("Virtual Agents Framework/Rename sample~ to sample")]
        static void RenameForDevelopment()
        {
             Directory.Move(path + "/Samples~", path + "/Samples");
            AssetDatabase.Refresh();
        }

    }
}