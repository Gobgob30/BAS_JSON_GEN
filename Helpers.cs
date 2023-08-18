using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ThunderRoad;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

namespace JSONGenerator.Helper
{
    public static class Helpers
    {
        public static bool FoldoutButton(bool foldout, string label)
        {
            bool result = foldout;
            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayout.DropdownButton(new GUIContent(label), FocusType.Passive))
            {
                result = !result;
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }
    }
    public class ReorderListHelpers
    {
        private float width;
        private float currentPos;
        private float x;
        private float y;
        private int numItems;
        private float increment;
        private float next_increment;
        public Rect[] rects;
        public ReorderListHelpers(float x, float y, float width, int numItems)
        {
            this.increment = width / numItems;
            this.next_increment = increment;
            this.currentPos = 0;
            this.width = width;
            this.numItems = numItems;
            this.x = x;
            this.y = y;
            this.rects = new Rect[numItems];
        }

        public void Setup(float[] array)
        {
            if (array.Length != numItems)
            {
                Debug.LogError("Array length does not match number of items");
                return;
            }
            for (int i = 0; i < array.Length; i++)
            {
                float value = array[i];
                if (value == 0)
                {
                    if (next_increment != increment)
                    {
                        value = next_increment;
                    }
                    else
                    {
                        value = increment;
                    }
                }
                rects[i] = new Rect(x + currentPos, y, value-10, EditorGUIUtility.singleLineHeight);
                currentPos += value;
                float temp = value - increment;
                next_increment = increment - temp;
            }
        }
    }
}
