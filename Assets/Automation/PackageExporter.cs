using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;


namespace InternalTools
{
    /// <summary>
    /// This class is used to package the Virtual Agents Framework for release.
    /// These methods will not be displayed when the package is used by other projects, as they are not in the Assets folder.
    /// It uses the C# class System.IO to rename the Samples folder to Samples~ and back.
    /// The SAMPLES_PACKAGED define is set and removed to enable/disable the tests that run on the samples.
    /// </summary>
    public class PackageExporter : EditorWindow
    {
        static string path = "Assets/Virtual Agents Framework";

        [MenuItem("Virtual Agents Framework/Package for release")]
        static void RenameForRelease()
        {
            UnityEngine.Debug.Log("1. Renaming \"Samples\" to \"Samples~\"");
            Directory.Move(path + "/Samples", path + "/Samples~");
            UnityEngine.Debug.Log("2. Deleting old Samples.meta");
            File.Delete(path + "/Samples.meta");

            UnityEngine.Debug.Log("3. Setting \"SAMPLES_PACKAGED\" define.");
            SetScriptDefine("SAMPLES_PACKAGED");
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Packaging finished. Unity should recompile soon...");
        }
        [MenuItem("Virtual Agents Framework/Unpackage for development")]
        static void RenameForDevelopment()
        {
            UnityEngine.Debug.Log("1. Renaming \"Samples~\" to \"Samples\"");
            Directory.Move(path + "/Samples~", path + "/Samples");
            // Unity will automatically create a new Samples.meta file, so we don't need to do that.
            // Samples~ doens't have a Samples.meta file, so we don't need to delete it.
            UnityEngine.Debug.Log("2. Removing \"SAMPLES_PACKAGED\" define.");
            RemoveScriptDefine("SAMPLES_PACKAGED");
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Unpacking finished. Unity should recompile soon...");
        }

        static void SetScriptDefine(string addDefine)
        {
            string[] defines;
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out defines);
            List<string> definesList = defines.ToList();
            //If the define is already set, remove all occurences of it and add it again
            definesList.RemoveAll(o => o == addDefine);
            //Add new define
            definesList.Add(addDefine);

            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, definesList.ToArray());
            //Check that everything went correctly
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out defines);
            DebugArray(defines);
        }

        static void RemoveScriptDefine(string removeDefine)
        {
            string[] defines;
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out defines);
            defines = defines.Where(o => o != removeDefine).ToArray();
            DebugArray(defines);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, defines);
        }

        static void DebugArray(string[] array)
        {
            string s = "Defines: ";
            foreach (string d in array)
            {
                s = s + d + ", ";
            }
            s = s.Substring(0, s.Length - 2);
            UnityEngine.Debug.Log(s);
        }
    }
}