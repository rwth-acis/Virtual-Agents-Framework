using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;


namespace InternalTools
{
    /// <summary>
    /// This class is used to prepare the Virtual Agents Framework for release.
    /// These methods will not be displayed when the package is used by other projects, as they are not in the Assets folder.
    /// It uses the C# class System.IO to rename the Samples folder to Samples~ and back.
    /// The SAMPLES_PACKAGED define is set and removed to enable/disable the tests that run on the samples.
    /// Based on the define, only the right method (Release/Development) will be displayed in the Unity Editor.
    /// </summary>
    public class PackageExporter : EditorWindow
    {
        static string path = "Assets/Virtual Agents Framework";

#if !SAMPLES_PACKAGED
        [MenuItem("Virtual Agents Framework/Prepare for release")]
        static void RenameForRelease()
        {
            bool confirm = EditorUtility.DisplayDialog("Prepare For Release: Save All Changes?", "In the process of preparing the package for release all changes will be saved. Are you sure you want to do that?", "Save All", "Do Not Save");
            if (!confirm)
            {
                UnityEngine.Debug.Log("Preparation for release cancelled.");
                return;
            }
            // Save all changes so that potential saves in the samples are not lost
            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log("1. Renaming \"Samples\" to \"Samples~\"");
            Directory.Move(path + "/Samples", path + "/Samples~");
            UnityEngine.Debug.Log("2. Deleting old Samples.meta");
            File.Delete(path + "/Samples.meta");

            UnityEngine.Debug.Log("3. Setting \"SAMPLES_PACKAGED\" define.");
            SetScriptDefine("SAMPLES_PACKAGED");
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Preparation for release finished. Unity should start to recompile soon...");
        }
#endif
#if SAMPLES_PACKAGED
        [MenuItem("Virtual Agents Framework/Prepare for development")]
        static void RenameForDevelopment()
        {
            bool confirm = EditorUtility.DisplayDialog("Prepare For Development: Save All Changes?", "In the process of preparing the package for development all changes will be saved. Are you sure you want to do that?", "Save All", "Do Not Save");
            if (!confirm)
            {
                UnityEngine.Debug.Log("Preparation for release cancelled.");
                return;
            }
            UnityEngine.Debug.Log("1. Renaming \"Samples~\" to \"Samples\"");
            Directory.Move(path + "/Samples~", path + "/Samples");
            // Unity will automatically create a new Samples.meta file, so we don't need to do that.
            // Samples~ doens't have a Samples.meta file, so we don't need to delete it.
            UnityEngine.Debug.Log("2. Removing \"SAMPLES_PACKAGED\" define.");
            RemoveScriptDefine("SAMPLES_PACKAGED"); // This will also save all assets to save the change in the project settings
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Preperation for development finished. Unity should start to recompile soon...");
        }
#endif

        static void SetScriptDefine(string addDefine)
        {
            string[] defines;
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out defines);
            List<string> definesList = defines.ToList();
            // If the define is already set, remove all occurences of it and add it again
            definesList.RemoveAll(o => o == addDefine);
            //Add new define
            definesList.Add(addDefine);

            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, definesList.ToArray());
            // Check that everything went correctly
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out defines);
            DebugArray(defines);
            AssetDatabase.SaveAssets();
        }

        static void RemoveScriptDefine(string removeDefine)
        {
            string[] defines;
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out defines);
            defines = defines.Where(o => o != removeDefine).ToArray();
            DebugArray(defines);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, defines);
            AssetDatabase.SaveAssets();
        }

        static void DebugArray(string[] array)
        {
            string s = "Defines:  ";
            foreach (string d in array)
            {
                s = s + d + ", ";
            }
            s = s.Substring(0, s.Length - 2);
            UnityEngine.Debug.Log(s);
        }
    }
}