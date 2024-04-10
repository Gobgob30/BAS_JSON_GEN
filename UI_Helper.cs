using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSONGenerator
{
    public static class UI_Generators
    {
        public static void FoldoutButton(ref bool foldout, string label)
        {
            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayout.DropdownButton(new GUIContent(label), FocusType.Passive, GUILayout.ExpandWidth(true)))
            {
                foldout = !foldout;
            }
            EditorGUILayout.EndHorizontal();
        }
        public static void FolderButton(ref string path, string display_text, float width, string default_path)
        {
            if (GUILayout.Button(path, new GUIStyle("textField"), GUILayout.Width(width)))
                path = EditorUtility.OpenFolderPanel(display_text, default_path, "");
        }
        public static void FileButton(ref string path, string display_text, float width, string default_path, string default_filename = "")
        {
            if (GUILayout.Button(path, new GUIStyle("textField"), GUILayout.Width(width)))
                path = EditorUtility.OpenFilePanel(display_text, default_path, default_filename);
        }
        public static void FileButtonWithFilter(ref string path, string display_text, float width, string default_path, string[] filters, string default_filename = "")
        {
            if (GUILayout.Button(path, new GUIStyle("textField"), GUILayout.Width(width)))
                path = EditorUtility.OpenFilePanelWithFilters(display_text, default_path, filters);
        }

    }
}
