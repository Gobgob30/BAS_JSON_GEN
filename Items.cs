using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSONGenerator
{
    public static class Items
    {
        // Constants
        private readonly static string[] file_filters = new string[] { "JSON files", "json", "All files", "*" };
        // Internal data
        private static ItemData itemData = new ItemData();
        private static Window instance = null;
        private static Vector2 scrollPosition = Vector2.zero;
        private static string filename = "";
        private static string savepath = "";
        private static string loadpath = "";
        // Drop downs
        private static bool basic = false;
        private static bool addresses = false;
        private static bool physicsInfo = false;
        private static bool advancedImbueInfo = false;
        private static bool damagerInfo = false;
        private static bool interactableInfo = false;
        private static bool moduleDrop = false;

        public static void OnGUI(Window window, Rect position)
        {
            if (instance == null)
                instance = window;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            UI_Generators.FoldoutButton(ref basic, "Basic Information");
            if (basic)
            {
                EditorGUI.indentLevel++;
                UI_Generators.FoldoutButton(ref moduleDrop, "");
                if (moduleDrop)
                {

                }
                EditorGUI.indentLevel--;
            }
            UI_Generators.FoldoutButton(ref addresses, "Addresses for assets");
            if (addresses)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }
            UI_Generators.FoldoutButton(ref physicsInfo, "Physics Info");
            if (physicsInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }
            UI_Generators.FoldoutButton(ref advancedImbueInfo, "Advanced Imbue Information");
            if (advancedImbueInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }
            UI_Generators.FoldoutButton(ref damagerInfo, "Damager Info");
            if (damagerInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }
            UI_Generators.FoldoutButton(ref interactableInfo, "Interactable Info");
            if (interactableInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            filename = EditorGUILayout.TextField("JSON Name", filename);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            float width = EditorGUIUtility.labelWidth / 4;
            UI_Generators.FolderButton(ref savepath, "Select folder to place JSON file.", width * 3, EditorPrefs.GetString("TRAB.GamePath") + "\\BladeAndSorcery_Data\\StreamingAssets\\Mods");
            if (GUILayout.Button("Save", GUILayout.Width(width)))
            {
                if (!savepath.IsNullOrEmptyOrWhitespace() && !filename.IsNullOrEmptyOrWhitespace())
                {
                    string name = filename.EndsWith(".json") ? filename : filename + ".json";
                    string json = JsonConvert.SerializeObject(itemData, Formatting.Indented);
                    if (!json.IsNullOrEmptyOrWhitespace())
                        File.WriteAllText(Path.Combine(savepath, name), json);
                }
            }
            width = (position.width - EditorGUIUtility.labelWidth) / 4;
            UI_Generators.FileButtonWithFilter(ref loadpath, "Select JSON file to load.", width * 3 - 16, EditorPrefs.GetString("TRAB.GamePath") + "\\BladeAndSorcery_Data\\StreamingAssets\\Mods", file_filters, filename);
            if (GUILayout.Button("Load", GUILayout.Width(width)))
            {

            }
            GUILayout.EndHorizontal();
        }
        public static void Refresh(GameObject rootPrefab)
        {

        }
    }
}