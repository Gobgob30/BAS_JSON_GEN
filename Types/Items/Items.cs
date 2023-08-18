using ThunderRoad;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using JSONGenerator.Helper;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UIElements;

namespace JSONGenerator
{
    public static class Items
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static string savepath = "Save Path";
        private static string loadpath = "Load Path";
        private static string filename = "";
        private static CompareWindow compareWindow;
        private static Window _instance;

        // Lists
        private static ReorderableList colliderGroups;
        private static ReorderableList damagerGroups;
        private static ReorderableList interactableGroups;
        private static ReorderableList whooshGroup;
        private static ReorderableList addressableGroup;
        // For addressableGroup
        private static int[] addressableGroup_selected_list;
        private static string[] addressableGroup_choices = new string[] { "None", "Prefab", "Icon", "Close Up Icon", "Snap Audio Container", "Unsnap Audio Container" };
        private static bool prefixAddress = false;
        private static string prefix = "";

        // Foldouts
        private static bool _basicInfo = true;
        private static bool _addresses = false;
        private static bool _physicsInfo = false;
        private static bool _advancedImbueInfo = false;
        private static bool _damagerInfo = false;
        private static bool _interactableInfo = false;
        private static bool _moduleDrop = false;
        private static List<bool> _rangeweapondataDrop = new List<bool>();
        private static List<bool> _rangeweapondataUse = new List<bool>();
        private static List<bool> _modulesDrop = new List<bool>();

        // Addressable Groups
        private static AddressableAssetGroup assetGroup;
        private static AddressableAssetGroup assetOld_Group;

        private static void runAsset(AddressableAssetEntry entry, int index)
        {
            int selected = 0;
            if (entry.address.ToLower().EndsWith(".icon"))
            {
                _instance._itemData.iconAddress = entry.address;
                selected = 2;
            }
            else if (entry.address.ToLower().EndsWith(".closeicon"))
            {
                _instance._itemData.closeUpIconAddress = entry.address;
                selected = 3;
            }
            else if (entry.address.ToLower().EndsWith(".prefab"))
            {
                _instance._itemData.prefabAddress = entry.address;
                selected = 1;
            }
            else if (entry.address.ToLower().EndsWith(".snap"))
            {
                _instance._itemData.snapAudioContainerAddress = entry.address;
                selected = 4;
            }
            else if (entry.address.ToLower().EndsWith(".unsnap"))
            {
                _instance._itemData.unsnapAudioContainerAddress = entry.address;
                selected = 5;
            }
            addressableGroup_selected_list[index] = selected;
        }

        private static void OnAssetChange(AddressableAssetGroup assetGroup)
        {
            AddressableAssetEntry[] entries = assetGroup.entries.ToArray();
            addressableGroup_selected_list = new int[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                AddressableAssetEntry entry = entries[i];
                runAsset(entry, i);
            }
        }

        private static void OnAssetChange(AddressableAssetGroup assetGroup, string Prefix)
        {
            AddressableAssetEntry[] entries = assetGroup.entries.ToArray();
            addressableGroup_selected_list = new int[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].address.StartsWith(Prefix.ToLower()))
                {
                    AddressableAssetEntry entry = entries[i];
                    runAsset(entry, i);
                }
            }
        }

        public static void OnGUI(Window instance, Rect position)
        {
            if (_instance == null)
                _instance = instance;
            if (_instance._itemData == null) _instance._itemData = new ItemData();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            _basicInfo = Helpers.FoldoutButton(_basicInfo, "Basic Info");
            if (_basicInfo)
            {
                EditorGUI.indentLevel++;
                _instance._itemData.id = EditorGUILayout.TextField("ID", _instance._itemData.id);
                _instance._itemData.version = EditorGUILayout.IntField("Version", _instance._itemData.version);
                _instance._itemData.localizationId = _instance._itemData.id;
                _instance._itemData.displayName = EditorGUILayout.TextField("Display Name", _instance._itemData.displayName);
                _instance._itemData.description = EditorGUILayout.TextField("Description", _instance._itemData.description);
                _instance._itemData.author = EditorGUILayout.TextField("Author", _instance._itemData.author);
                _instance._itemData.tier = EditorGUILayout.IntSlider("Tier", _instance._itemData.tier, 0, 4);
                _instance._itemData.price = EditorGUILayout.FloatField("Price", _instance._itemData.price);
                _instance._itemData.purchasable = EditorGUILayout.Toggle("Purchasable", _instance._itemData.purchasable);
                _instance._itemData.levelRequired = EditorGUILayout.IntSlider("Level Required ~Unused?", _instance._itemData.levelRequired, 0, 100);
                _instance._itemData.category = EditorGUILayout.TextField("Category", _instance._itemData.category);
                _instance._itemData.type = (ItemData.Type)EditorGUILayout.EnumPopup("Type", _instance._itemData.type);
                _instance._itemData.slot = EditorGUILayout.TextField("Slot", _instance._itemData.slot);
                _instance._itemData.drainImbueOnSnap = EditorGUILayout.Toggle("Drain Imbue Holstered", _instance._itemData.drainImbueOnSnap);
                _instance._itemData.manaRegenMultiplier = EditorGUILayout.FloatField("Mana Regen Multiplier", _instance._itemData.manaRegenMultiplier);
                _instance._itemData.spellChargeSpeedPlayerMultiplier = EditorGUILayout.FloatField("Spell Charge Speed Player Multiplier", _instance._itemData.spellChargeSpeedPlayerMultiplier);
                _instance._itemData.spellChargeSpeedPlayerMultiplier = EditorGUILayout.FloatField("Spell Charge Speed Player Multiplier", _instance._itemData.spellChargeSpeedPlayerMultiplier);
                _instance._itemData.grippable = EditorGUILayout.Toggle("Grippable", _instance._itemData.grippable);
                _instance._itemData.flyFromThrow = EditorGUILayout.Toggle("Fly From Throw", _instance._itemData.flyFromThrow);
                if (_instance._itemData.modules == null)
                {
                    _instance._itemData.modules = new List<ItemModule>();
                }
                _moduleDrop = EditorGUILayout.Foldout(_moduleDrop, "Modules");
                if (_moduleDrop)
                {
                    EditorGUI.indentLevel++;
                    if (_instance._itemData.modules.Count > _modulesDrop.Count)
                    {
                        for (int i = _modulesDrop.Count; i < _instance._itemData.modules.Count; i++)
                        {
                            _modulesDrop.Add(false);
                            _rangeweapondataDrop.Add(false);
                            _rangeweapondataUse.Add(false);
                        }
                    }
                    for (int i = 0; i < _instance._itemData.modules.Count; i++)
                    {
                        _modulesDrop[i] = EditorGUILayout.Foldout(_modulesDrop[i], "Module " + i);
                        if (_modulesDrop[i])
                        {
                            EditorGUI.indentLevel++;
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                _instance._itemData.modules.RemoveAt(i);
                                _modulesDrop.RemoveAt(i);
                                _rangeweapondataDrop.RemoveAt(i);
                            }
                            ItemModuleAI aI = _instance._itemData.modules[i] as ItemModuleAI;
                            if (aI == null)
                                aI = new ItemModuleAI();
                            aI.weaponClass = (ItemModuleAI.WeaponClass)EditorGUILayout.EnumPopup("Weapon Class", aI.weaponClass);
                            aI.weaponHandling = (ItemModuleAI.WeaponHandling)EditorGUILayout.EnumPopup("Weapon Handling", aI.weaponHandling);
                            aI.secondaryClass = (ItemModuleAI.WeaponClass)EditorGUILayout.EnumPopup("Secondary Class", aI.secondaryClass);
                            aI.secondaryHandling = (ItemModuleAI.WeaponHandling)EditorGUILayout.EnumPopup("Secondary Handling", aI.secondaryHandling);
                            aI.parryRotation = EditorGUILayout.FloatField("Parry Rotation", aI.parryRotation);
                            aI.parryDualRotation = EditorGUILayout.FloatField("Parry Dual Rotation", aI.parryDualRotation);
                            aI.armResistanceMultiplier = EditorGUILayout.FloatField("Arm Resistance Multiplier", aI.armResistanceMultiplier);
                            aI.parryRevertAngleRange = EditorGUILayout.Vector2Field("Parry Revert Angle Range", aI.parryRevertAngleRange);
                            aI.parryDefaultPosition = EditorGUILayout.Vector3Field("Parry Default Position", aI.parryDefaultPosition);
                            aI.parryDefaultLeftRotation = EditorGUILayout.Vector3Field("Parry Default Left Rotation", aI.parryDefaultLeftRotation);
                            aI.parryDefaultRightRotation = EditorGUILayout.Vector3Field("Parry Default Right Rotation", aI.parryDefaultRightRotation);
                            aI.allowDynamicHeight = EditorGUILayout.Toggle("Allow Dynamic Height", aI.allowDynamicHeight);
                            aI.defenseHasPriority = EditorGUILayout.Toggle("Defense Has Priority", aI.defenseHasPriority);
                            aI.attackIgnore = EditorGUILayout.Toggle("Attack Ignore", aI.attackIgnore);
                            aI.attackForceParryIgnoreRotation = EditorGUILayout.Toggle("Attack Force Parry Ignore Rotation", aI.attackForceParryIgnoreRotation);
                            aI.ammoType = EditorGUILayout.TextField("Ammo Type", aI.ammoType);
                            _rangeweapondataUse[i] = EditorGUILayout.Toggle("Include Range Weapon Data -Will Delete all changes-", _rangeweapondataUse[i]);
                            bool old_gui = GUI.enabled;
                            GUI.enabled = _rangeweapondataUse[i];
                            _rangeweapondataDrop[i] = EditorGUILayout.Foldout(_rangeweapondataDrop[i], "Range Weapon Data");
                            if (_rangeweapondataDrop[i] && _rangeweapondataUse[i])
                            {
                                EditorGUI.indentLevel++;
                                if (aI.rangedWeaponData == null)
                                {
                                    aI.rangedWeaponData = new ItemModuleAI.RangedWeaponData();
                                }
                                aI.rangedWeaponData.tooCloseDistance = EditorGUILayout.FloatField("Too Close Distance", aI.rangedWeaponData.tooCloseDistance);
                                aI.rangedWeaponData.spread = EditorGUILayout.Vector2Field("Spread", aI.rangedWeaponData.spread);
                                aI.rangedWeaponData.projectileSpeed = EditorGUILayout.FloatField("Projectile Speed", aI.rangedWeaponData.projectileSpeed);
                                aI.rangedWeaponData.accountForGravity = EditorGUILayout.Toggle("Account For Gravity", aI.rangedWeaponData.accountForGravity);
                                aI.rangedWeaponData.weaponAimAngleOffset = EditorGUILayout.Vector3Field("Weapon Aim Angle Offset", aI.rangedWeaponData.weaponAimAngleOffset);
                                aI.rangedWeaponData.weaponHoldPositionOffset = EditorGUILayout.Vector3Field("Weapon Hold Position Offset", aI.rangedWeaponData.weaponHoldPositionOffset);
                                aI.rangedWeaponData.weaponHoldAngleOffset = EditorGUILayout.Vector3Field("Weapon Hold Angle Offset", aI.rangedWeaponData.weaponHoldAngleOffset);
                                aI.rangedWeaponData.customRangedAttackAnimationData = EditorGUILayout.TextField("Custom Ranged Attack Animation Data", aI.rangedWeaponData.customRangedAttackAnimationData);
                                EditorGUI.indentLevel--;
                            }
                            if (aI.rangedWeaponData != null && !_rangeweapondataUse[i])
                            {
                                aI.rangedWeaponData = null;
                            }
                            GUI.enabled = old_gui;
                            _instance._itemData.modules[i] = aI;
                            EditorGUI.indentLevel--;
                        }
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Module"))
                    {
                        _instance._itemData.modules.Add(new ItemModuleAI());
                        _modulesDrop.Add(false);
                        _rangeweapondataDrop.Add(false);
                        _rangeweapondataUse.Add(false);
                    }
                    GUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            _addresses = Helpers.FoldoutButton(_addresses, "Addresses");
            if (_addresses)
            {
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                prefixAddress = EditorGUILayout.Toggle("Use Prefix", prefixAddress, GUILayout.Width(EditorGUIUtility.labelWidth));
                bool old = GUI.enabled;
                GUI.enabled = prefixAddress;
                prefix = EditorGUILayout.TextField("Prefix - Case Sensitive", prefix);
                GUI.enabled = old;
                GUILayout.EndHorizontal();
                assetGroup = (AddressableAssetGroup)EditorGUILayout.ObjectField("Addressable Group", assetGroup, typeof(AddressableAssetGroup), true);
                if (assetGroup != null)
                {
                    if (assetGroup != assetOld_Group)
                    {
                        assetOld_Group = assetGroup;
                        if (prefix == "")
                            OnAssetChange(assetGroup);
                        else
                            OnAssetChange(assetGroup, prefix);
                    }
                    if (addressableGroup == null)
                    {
                        addressableGroup = new ReorderableList(assetGroup.entries.ToList(), typeof(AddressableAssetEntry), false, true, false, false);
                        addressableGroup.drawHeaderCallback = (Rect rect) =>
                        {
                            ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                            reorder.Setup(new float[] { 0f, 0f, 0f });
                            EditorGUI.LabelField(reorder.rects[0], "Addressable Groups");
                            EditorGUI.LabelField(reorder.rects[1], "Address");
                            EditorGUI.LabelField(reorder.rects[2], "Target");
                        };
                        addressableGroup.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                        {
                            AddressableAssetEntry entry = assetGroup.entries.ToList()[index];
                            int selected = addressableGroup_selected_list[index];
                            ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                            reorder.Setup(new float[] { 0f, 0f, 0f });
                            EditorGUI.LabelField(reorder.rects[0], "Entry #" + index);
                            EditorGUI.LabelField(reorder.rects[1], entry.address);
                            selected = EditorGUI.Popup(reorder.rects[2], selected, addressableGroup_choices);
                            addressableGroup_selected_list[index] = selected;
                            switch (selected)
                            {
                                case 0:
                                    break;
                                case 1:
                                    _instance._itemData.prefabAddress = entry.address;
                                    break;
                                case 2:
                                    _instance._itemData.iconAddress = entry.address;
                                    break;
                                case 3:
                                    _instance._itemData.closeUpIconAddress = entry.address;
                                    break;
                                case 4:
                                    _instance._itemData.snapAudioContainerAddress = entry.address;
                                    break;
                                case 5:
                                    _instance._itemData.unsnapAudioContainerAddress = entry.address;
                                    break;
                            }
                        };
                    }
                    addressableGroup.DoLayoutList();
                }
                _instance._itemData.prefabAddress = EditorGUILayout.TextField("Prefab Address", _instance._itemData.prefabAddress);
                _instance._itemData.iconAddress = EditorGUILayout.TextField("Icon Address", _instance._itemData.iconAddress);
                _instance._itemData.closeUpIconAddress = EditorGUILayout.TextField("Close Up Icon Address", _instance._itemData.closeUpIconAddress);
                _instance._itemData.snapAudioContainerAddress = EditorGUILayout.TextField("Snap Audio Container Address", _instance._itemData.snapAudioContainerAddress);
                _instance._itemData.unsnapAudioContainerAddress = EditorGUILayout.TextField("Unsnap Audio Container Address", _instance._itemData.unsnapAudioContainerAddress);
                EditorGUI.indentLevel--;
            }
            _physicsInfo = Helpers.FoldoutButton(_physicsInfo, "Physics Info");
            if (_physicsInfo)
            {
                EditorGUI.indentLevel++;
                _instance._itemData.mass = EditorGUILayout.FloatField("Mass", _instance._itemData.mass);
                _instance._itemData.drag = EditorGUILayout.FloatField("Drag", _instance._itemData.drag);
                _instance._itemData.angularDrag = EditorGUILayout.FloatField("Angular Drag", _instance._itemData.angularDrag);
                if (colliderGroups == null)
                {
                    colliderGroups = new ReorderableList(_instance._itemData.colliderGroups, typeof(ColliderGroup), false, true, true, true);
                    colliderGroups.drawHeaderCallback = (Rect rect) =>
                    {
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                        reorder.Setup(new float[] { 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "Collider Groups");
                        EditorGUI.LabelField(reorder.rects[1], "Transform Name");
                        EditorGUI.LabelField(reorder.rects[2], "Collider Type");
                    };
                    colliderGroups.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        ItemData.ColliderGroup colliderGroup = _instance._itemData.colliderGroups[index];
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                        reorder.Setup(new float[] { 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "ColliderGroup #" + index.ToString());
                        colliderGroup.transformName = EditorGUI.TextField(reorder.rects[1], colliderGroup.transformName);
                        colliderGroup.colliderGroupId = EditorGUI.TextField(reorder.rects[2], colliderGroup.colliderGroupId);
                    };
                }
                colliderGroups.DoLayoutList();
                _instance._itemData.waterHandSpringMultiplierCurve = EditorGUILayout.CurveField("Water Hand Spring Multiplier Curve", _instance._itemData.waterHandSpringMultiplierCurve);
                _instance._itemData.waterDragMultiplierCurve = EditorGUILayout.CurveField("Water Hand Damper Multiplier Curve", _instance._itemData.waterDragMultiplierCurve);
                _instance._itemData.waterSampleMinRadius = EditorGUILayout.FloatField("Water Sample Min Radius", _instance._itemData.waterSampleMinRadius);
                _instance._itemData.flyRotationSpeed = EditorGUILayout.FloatField("Fly Rotation Speed", _instance._itemData.flyRotationSpeed);
                _instance._itemData.flyThrowAngle = EditorGUILayout.FloatField("Fly Throw Angle", _instance._itemData.flyThrowAngle);
                EditorGUI.indentLevel--;
            }
            _advancedImbueInfo = Helpers.FoldoutButton(_advancedImbueInfo, "Advanced Imbue Info");
            if (_advancedImbueInfo)
            {
                EditorGUI.indentLevel++;
                _instance._itemData.imbueEnergyOverTimeOnSnap = EditorGUILayout.CurveField("Imbue Over Time Hostered", _instance._itemData.imbueEnergyOverTimeOnSnap);
                EditorGUI.indentLevel--;
            }
            _damagerInfo = Helpers.FoldoutButton(_damagerInfo, "Damager Info");
            if (_damagerInfo)
            {
                EditorGUI.indentLevel++;
                if (damagerGroups == null)
                {
                    damagerGroups = new ReorderableList(_instance._itemData.damagers, typeof(Damager), false, true, true, true);
                    damagerGroups.drawHeaderCallback = (Rect rect) =>
                    {
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                        reorder.Setup(new float[] { 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "Damager Groups");
                        EditorGUI.LabelField(reorder.rects[1], "Transform Name");
                        EditorGUI.LabelField(reorder.rects[2], "Damager Type");
                    };
                    damagerGroups.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        ItemData.Damager damagerGroup = _instance._itemData.damagers[index];
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                        reorder.Setup(new float[] { 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "DamagerGroup #" + index.ToString());
                        damagerGroup.transformName = EditorGUI.TextField(reorder.rects[1], damagerGroup.transformName);
                        damagerGroup.damagerID = EditorGUI.TextField(reorder.rects[2], damagerGroup.damagerID);
                    };
                }
                damagerGroups.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            _interactableInfo = Helpers.FoldoutButton(_interactableInfo, "Interactable Info");
            if (_interactableInfo)
            {
                EditorGUI.indentLevel++;
                if (interactableGroups == null)
                {
                    interactableGroups = new ReorderableList(_instance._itemData.Interactables, typeof(ItemData.Interactable), false, true, true, true);
                    interactableGroups.drawHeaderCallback = (Rect rect) =>
                    {
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                        reorder.Setup(new float[] { 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "Interactable Groups");
                        EditorGUI.LabelField(reorder.rects[1], "Transform Name");
                        EditorGUI.LabelField(reorder.rects[2], "Interactable Type");
                    };
                    interactableGroups.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        ItemData.Interactable interactableGroup = _instance._itemData.Interactables[index];
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 3);
                        reorder.Setup(new float[] { 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "InteractableGroup #" + index.ToString());
                        interactableGroup.transformName = EditorGUI.TextField(reorder.rects[1], interactableGroup.transformName);
                        interactableGroup.interactableId = EditorGUI.TextField(reorder.rects[2], interactableGroup.interactableId);
                    };
                }
                interactableGroups.DoLayoutList();
                if (whooshGroup == null)
                {
                    whooshGroup = new ReorderableList(_instance._itemData.whooshs, typeof(ItemData.Whoosh), false, true, true, true);
                    whooshGroup.drawHeaderCallback = (Rect rect) =>
                    {
                        ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 8);
                        reorder.Setup(new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f });
                        EditorGUI.LabelField(reorder.rects[0], "Whoosh Groups");
                        EditorGUI.LabelField(reorder.rects[1], "Transform Name");
                        EditorGUI.LabelField(reorder.rects[2], "Whoosh ID");
                        EditorGUI.LabelField(reorder.rects[3], "Trigger");
                        EditorGUI.LabelField(reorder.rects[4], "Stop on Snap");
                        EditorGUI.LabelField(reorder.rects[5], "Min Velocity");
                        EditorGUI.LabelField(reorder.rects[6], "Max Velocity");
                        EditorGUI.LabelField(reorder.rects[7], "Dampening");
                    };
                }
                whooshGroup.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    ItemData.Whoosh whooshGroup = _instance._itemData.whooshs[index];
                    ReorderListHelpers reorder = new ReorderListHelpers(rect.x, rect.y, rect.width, 8);
                    reorder.Setup(new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f });
                    EditorGUI.LabelField(reorder.rects[0], "WhooshGroup #" + index.ToString());
                    whooshGroup.transformName = EditorGUI.TextField(reorder.rects[1], whooshGroup.transformName);
                    whooshGroup.effectId = EditorGUI.TextField(reorder.rects[2], whooshGroup.effectId);
                    whooshGroup.trigger = (WhooshPoint.Trigger)EditorGUI.EnumPopup(reorder.rects[3], whooshGroup.trigger);
                    whooshGroup.stopOnSnap = EditorGUI.Toggle(reorder.rects[4], whooshGroup.stopOnSnap);
                    whooshGroup.minVelocity = EditorGUI.FloatField(reorder.rects[5], whooshGroup.minVelocity);
                    whooshGroup.maxVelocity = EditorGUI.FloatField(reorder.rects[6], whooshGroup.maxVelocity);
                    whooshGroup.dampening = EditorGUI.FloatField(reorder.rects[7], whooshGroup.dampening);
                };
                whooshGroup.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            #region Section for bottom buttons
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            filename = EditorGUILayout.TextField("JSON Name", filename);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            float width = EditorGUIUtility.labelWidth / 4;
            if (GUILayout.Button(savepath, new GUIStyle("textField"), GUILayout.Width(width * 3)))
            {
                savepath = EditorUtility.OpenFolderPanel("Select folder to place JSON file.", EditorPrefs.GetString("TRAB.GameExePath"), "");
            }
            if (GUILayout.Button("Save", GUILayout.Width(width)))
            {
                SaveItemData(savepath, filename);
            }
            width = (position.width - EditorGUIUtility.labelWidth) / 4;
            if (GUILayout.Button(loadpath, new GUIStyle("textField"), GUILayout.Width(width * 3 - 16)))
            {
                loadpath = EditorUtility.OpenFilePanel("Select JSON file to load.", EditorPrefs.GetString("TRAB.GameExePath"), "json");
            }
            if (GUILayout.Button("Load", GUILayout.Width(width)))
            {
                LoadItemData(_instance, loadpath);
            }
            GUILayout.EndHorizontal();
            #endregion
        }

        private static void LoadItemData(Window instance, string path)
        {
            if (path.IsNullOrEmptyOrWhitespace()) return;
            string json = File.ReadAllText(path);
            ItemData new_itemData = JsonConvert.DeserializeObject<ItemData>(json);
            if (new_itemData == null) return;
            compareWindow = CompareWindow.ShowWindow(instance, new_itemData, instance._itemData);
        }

        private static void SaveItemData(string path, string fileName)
        {
            if (path.IsNullOrEmptyOrWhitespace()) return;
            string json = JsonConvert.SerializeObject(_instance._itemData, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, fileName.IsNullOrEmptyOrWhitespace() ? _instance._itemData.displayName + ".json" : fileName), json);
        }

        public static void OnGameObjectUpdate(GameObject _rootPrefab)
        {
            if (_instance._itemData == null) return;
            Item item = _rootPrefab.GetComponent<Item>();
            if (item == null) { Debug.Log("Item is null"); return; }
            _instance._itemData.id = item.itemId;
            _instance._itemData.displayName = _rootPrefab.name;
            ColliderGroup[] colliderGroups = _rootPrefab.GetComponentsInChildren<ColliderGroup>();
            foreach (ColliderGroup c in colliderGroups)
            {
                ItemData.ColliderGroup colliderGroup = new ItemData.ColliderGroup();
                colliderGroup.transformName = c.transform.name;
                _instance._itemData.colliderGroups.Add(colliderGroup);
            }
            Rigidbody rigidbody = _rootPrefab.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                _instance._itemData.mass = rigidbody.mass;
                _instance._itemData.drag = rigidbody.drag;
                _instance._itemData.angularDrag = rigidbody.angularDrag;
            }
            Damager[] damagers = _rootPrefab.GetComponentsInChildren<Damager>();
            foreach (Damager d in damagers)
            {
                ItemData.Damager damager = new ItemData.Damager();
                damager.transformName = d.transform.name;
                _instance._itemData.damagers.Add(damager);
            }
            Interactable[] interactables = _rootPrefab.GetComponentsInChildren<Interactable>();
            foreach (Interactable i in interactables)
            {
                ItemData.Interactable interactable = new ItemData.Interactable();
                interactable.transformName = i.transform.name;
                _instance._itemData.Interactables.Add(interactable);
            }
            WhooshPoint[] whooshPoints = _rootPrefab.GetComponentsInChildren<WhooshPoint>();
            foreach (WhooshPoint w in whooshPoints)
            {
                ItemData.Whoosh whooshPoint = new ItemData.Whoosh();
                whooshPoint.transformName = w.transform.name;
                _instance._itemData.whooshs.Add(whooshPoint);
            }
        }

        public static string OnGenerate()
        {
            if (_instance._itemData == null)
            {
                EditorUtility.DisplayDialog("Item Data Null", "The Item data was null. Try again.", "Done");
                return "";
            }
            return JsonConvert.SerializeObject(_instance._itemData, Formatting.Indented);
        }

    }
}
