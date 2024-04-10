using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif  


public class Window : EditorWindow
{
    private static float window_minimum;
    private static GameObject _rootPrefab = null;
    private static bool _rootPrefabRefresh = false;
    private static GameObject _oldRootPrefab = null;
    private static string[] _jsonTypes = new string[] { "Item", "Level", "Spell", "Effect", "Creature" };
    private static int _jsonTypeIndex = 0;
    private static string _jsonType = _jsonTypes[_jsonTypeIndex];

    public Window instance;

    [MenuItem("ThunderRoad (SDK)/Tools/JSON Generator")]
    [MenuItem("SeaMortal/JSON Generator")]
    public static void ShowWindow()
    {
        Window window = EditorWindow.GetWindow<Window>("JSON Generator");
    }

    void OnGUI()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (this.minSize.x != window_minimum || this.minSize.y != window_minimum)
        {
            this.minSize = new Vector2(window_minimum, window_minimum);
        }
        EditorGUIUtility.labelWidth = position.width / 3;
        _jsonTypeIndex = EditorGUILayout.Popup("JSON Type", _jsonTypeIndex, _jsonTypes);
        _jsonType = _jsonTypes[_jsonTypeIndex];
        _rootPrefab = (GameObject)EditorGUILayout.ObjectField("Root Prefab", _rootPrefab, typeof(GameObject), true);
        if (_rootPrefab == null) { GUI.enabled = false; return; }
        if (_rootPrefab != _oldRootPrefab)
        {
            _rootPrefabRefresh = true;
            _oldRootPrefab = _rootPrefab;
        }
        switch (_jsonTypeIndex)
        {
            case 0:
                if (_rootPrefabRefresh)
                {
                    _rootPrefabRefresh = false;
                    JSONGenerator.Items.Refresh(_rootPrefab);
                }
                JSONGenerator.Items.OnGUI(instance, position);
                break;
            case 1:
                JSONGenerator.Levels.OnGUI(instance, position);
                break;
            case 2:
                JSONGenerator.Spells.OnGUI(instance, position);
                break;
            case 3:
                JSONGenerator.Effects.OnGUI(instance, position);
                break;
            case 4:
                JSONGenerator.Creatures.OnGUI(instance, position);
                break;
        }
        GUI.enabled = true;
    }
}