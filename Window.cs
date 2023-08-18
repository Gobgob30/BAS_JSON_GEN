using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;
using JSONGenerator.Helper;

#if UNITY_EDITOR
using UnityEditor;
#endif  

public class Window : EditorWindow
{
    public Window instance;
    private float _min = 600;
    // JSON Types
    private string[] _jsonTypes = new string[] { "Item", "Level", "Spell", "Effect", "Creature" };
    private static int _jsonTypeIndex = 0;
    // Prefabs
    private GameObject _rootPrefab = null;
    private GameObject _rootOldPrefab = null;
    // Data Objects
    public ItemData _itemData = null;
    private LevelData _levelData = null;
    private SpellData _spellData = null;
    private EffectData _effectData = null;
    private CreatureData _creatureData = null;
    [MenuItem("ThunderRoad (SDK)/Tools/JSON Generator")]
    [MenuItem("SeaMortal/JSON Generator")]
    public static void ShowWindow()
    {
        Window window = EditorWindow.GetWindow<Window>("JSON Generator");
    }

    private void OnEnable()
    {
        ThunderRoad.AssetBundleBuilder.OnBuildEvent += (a) =>
        {
            EditorUtility.DisplayDialog("Build Time", "The build took " + a.ToString() + " seconds", "Done");
            ShowWindow();
        };
    }

    void OnGUI()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (this.minSize.x != _min || this.minSize.y != _min)
        {
            this.minSize = new Vector2(_min, _min);
        }
        EditorGUIUtility.labelWidth = position.width / 3;
        _rootPrefab = (GameObject)EditorGUILayout.ObjectField("Root Prefab", _rootPrefab, typeof(GameObject), true);
        _jsonTypeIndex = EditorGUILayout.Popup("JSON Type", _jsonTypeIndex, _jsonTypes);
        if (_rootPrefab == null) GUI.enabled = false;
        switch (_jsonTypeIndex)
        {
            case 0:
                JSONGenerator.Items.OnGUI(instance, position);
                break;
            case 1:
                _levelData = new LevelData();
                break;
            case 2:
                _spellData = new SpellData();
                break;
            case 3:
                _effectData = new EffectData();
                break;
            case 4:
                _creatureData = new CreatureData();
                break;
        }
        if (_rootPrefab != _rootOldPrefab)
        {
            _rootOldPrefab = _rootPrefab;
            switch (_jsonTypeIndex)
            {
                case 0:
                    JSONGenerator.Items.OnGameObjectUpdate(_rootPrefab);
                    break;
            }
        }
        GUI.enabled = true;
    }
}
