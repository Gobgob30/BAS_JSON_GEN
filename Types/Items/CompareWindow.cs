using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;
using JSONGenerator.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif  

public class CompareWindow : EditorWindow
{
    public ItemData new_itemData;
    public bool done = false;
    public bool doneGenerating = false;
    public Dictionary<string, object> old_values;
    public Dictionary<string, object> new_values;

    private Window parentwindow;
    private CompareWindow window;
    private bool[] _shouldOveride;
    private string[] _key; // = new string[] { "id", "sensitiveContent", "sensitiveFilterBehaviour", "version", "localizationId", "displayName", "description", "author", "price", "purchasable", "tier", "weight", "size", "levelRequired", "category", "iconEffectId", "preferredItemCenter", "prefabAddress", "iconAddress", "closeUpIconAddress", "pooledCount", "androidPooledCount", "type", "canBeStoredInPlayerInventory", "limitMaxStorableQuantity", "maxStorableQuantity", "isStackable", "maxStackQuantity", "inventoryHoverSounds", "inventorySelectSounds", "inventoryStoreSounds", "slot", "snapAudioContainerAddress", "snapAudioVolume_dB", "unsnapAudioContainerAddress", "unsnapAudioVolume_dB", "overrideMassAndDrag", "mass", "drag", "angularDrag", "manaRegenMultiplier", "spellChargeSpeedPlayerMultiplier", "spellChargeSpeedNPCMultiplier", "collisionMaxOverride", "collisionEnterOnly", "collisionNoMinVelocityCheck", "forceLayer", "waterHandSpringMultiplierCurve", "waterDragMultiplierCurve", "waterSampleMinRadius", "flyFromThrow", "flyRotationSpeed", "flyThrowAngle", "telekinesisSafeDistance", "telekinesisSpinEnabled", "telekinesisThrowRatio", "telekinesisAutoGrabAnyHandle", "grippable", "grabAndGripClimb", "playerGrabAndGripChangeLayer", "customSnaps", "drainImbueOnSnap", "imbueEnergyOverTimeOnSnap", "modules", "colliderGroups", "damagers", "Interactables", "effectHinges", "whooshs", };
    private string[] _defualtOverides = new string[] { "id", "localizationId", "displayName", "prefabAddress", "iconAddress", "closeUpIconAddress", "customSnaps", "colliderGroups", "damagers", "Interactables", "effectHinges", "whooshs", };
    private Vector2 scroll_position = Vector2.zero;
    private float _min = 800;

    public static CompareWindow ShowWindow(Window instance, ItemData new_itemData, ItemData old_itemData)
    {
        CompareWindow window = EditorWindow.GetWindow<CompareWindow>("Compare Window");
        window.window = window;
        window.parentwindow = instance;
        window.new_itemData = old_itemData;
        System.Type new_itemType = new_itemData.GetType();
        System.Type old_itemType = old_itemData.GetType();
        System.Reflection.FieldInfo[] fields = old_itemType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        int length = fields.Length;
        window.old_values = new Dictionary<string, object>(length);
        window.new_values = new Dictionary<string, object>(length);
        window._shouldOveride = new bool[length];
        List<string> keyList = new List<string>(length);
        for (int i = 0; i < fields.Length; i++)
        {
            keyList.Add(fields[i].Name);
            window.old_values.Add(fields[i].Name, fields[i].GetValue(old_itemData));
            window.new_values.Add(fields[i].Name, fields[i].GetValue(new_itemData));
            if (window._defualtOverides.Contains(fields[i].Name))
                window._shouldOveride[i] = true;
            else
                window._shouldOveride[i] = false;
        }
        window._key = keyList.ToArray();
        //if (window._key == null)
        //{
        //    window.done = true;
        //    window.Close();
        //}
        //for (int i = 0; i < window._key.Length; i++)
        //{
        //    string key = window._key[i];
        //    window.new_values[key] = new_itemType.GetField(key).GetValue(new_itemData);
        //    window.old_values[key] = old_itemType.GetField(key).GetValue(old_itemData);
        //    if (window._defualtOverides.Contains(key))
        //        window._shouldOveride[i] = true;
        //    else
        //        window._shouldOveride[i] = false;
        //}
        window.done = false;
        window.doneGenerating = true;
        return window;
    }

    public void OnGUI()
    {
        if (old_values == null || new_values == null)
        {
            Debug.LogError("Old or New value is null: " + old_values.ToString() + " " + new_values.ToString());
            done = true;
            Close();
        }
        if (this.minSize.x != _min || this.minSize.y != _min)
        {
            this.minSize = new Vector2(_min, _min);
        }
        scroll_position = GUILayout.BeginScrollView(scroll_position);
        float width = position.width / 3;
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        float toggleWidth = 150;
        EditorGUILayout.LabelField("Old Item", GUILayout.Width(width - 20));
        EditorGUILayout.LabelField("Old overide New", GUILayout.Width(toggleWidth - 20));
        EditorGUILayout.LabelField("New Item", GUILayout.Width((width * 2 - toggleWidth) - 20));
        GUILayout.EndHorizontal();
        for (int i = 0; i < _key.Length; i++)
        {
            if (old_values[_key[i]] == null)
                continue;
            if (new_values[_key[i]] == null)
                continue;
            EditorGUILayout.BeginHorizontal();
            if (!old_values[_key[i]].Equals(new_values[_key[i]]))
            {
                if (_shouldOveride[i])
                    GUI.color = Color.green;
                else
                    GUI.color = Color.yellow;
            }
            EditorGUILayout.LabelField(_key[i] + ": " + old_values[_key[i]].ToString(), GUILayout.Width(width - 20));
            GUI.color = Color.white;
            _shouldOveride[i] = EditorGUILayout.Toggle(_shouldOveride[i], GUILayout.Width(toggleWidth - 20));
            if (!old_values[_key[i]].Equals(new_values[_key[i]]))
            {
                if (_shouldOveride[i])
                    GUI.color = Color.yellow;
                else
                    GUI.color = Color.green;
            }
            EditorGUILayout.LabelField(_key[i] + ": " + new_values[_key[i]].ToString(), GUILayout.Width((width * 2 - toggleWidth) - 20));
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Apply", GUILayout.Width(position.width)))
        {
            Update_itemData();
        }
        GUILayout.EndHorizontal();
    }

    public void Update_itemData()
    {
        for (int i = 0; i < _key.Length; i++)
        {
            if (!_shouldOveride[i])
            {
                if (new_values[_key[i]] == null) continue;
                new_itemData.GetType().GetField(_key[i]).SetValue(new_itemData, new_values[_key[i]]);
            }
        }
        parentwindow._itemData = new_itemData;
        Close();
    }
}
