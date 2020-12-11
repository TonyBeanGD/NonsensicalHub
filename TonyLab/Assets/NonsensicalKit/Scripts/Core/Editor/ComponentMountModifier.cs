using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NonsensicalKit.Editor
{

    public class ComponentMountModifier : EditorWindow
    {
        [MenuItem("TBTools/组件挂载修改器")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ComponentMountModifier));
        }

        private static class ComponentMountModifierPanel
        {
            public static int ComponentCount;

            public static int ApplyCount;

            public static bool[] isMount;

            public static MonoScript[] components;
        }

        private void OnGUI()
        {

            ComponentMountModifierPanel.ComponentCount = EditorGUILayout.IntField("组件数量", ComponentMountModifierPanel.ComponentCount, GUILayout.MinWidth(100f));

            if (GUILayout.Button("应用数量"))
            {
                ComponentMountModifierPanel.ApplyCount = ComponentMountModifierPanel.ComponentCount;
                ComponentMountModifierPanel.isMount = new bool[ComponentMountModifierPanel.ApplyCount];
                ComponentMountModifierPanel.components = new MonoScript[ComponentMountModifierPanel.ApplyCount];
            }

            for (int i = 0; i < ComponentMountModifierPanel.ApplyCount; i++)
            {
                ComponentMountModifierPanel.components[i] = (MonoScript)EditorGUILayout.ObjectField("组件", ComponentMountModifierPanel.components[i], typeof(MonoScript), false, GUILayout.MinWidth(100f));
                ComponentMountModifierPanel.isMount[i] = EditorGUILayout.Toggle("是否挂载", ComponentMountModifierPanel.isMount[i], GUILayout.MinWidth(100f));
            }

            if (GUILayout.Button("应用"))
            {
                Debug.Log(1);
                for (int i = 0; i < ComponentMountModifierPanel.components.Length; i++)
                {
                    Debug.Log(2);
                    UnityEngine.Object[] tArray = Resources.FindObjectsOfTypeAll(typeof(Transform));

                    foreach (var item in tArray)
                    {
                        Transform temp = item as Transform;

                        Undo.RecordObject(temp, temp.gameObject.name);
                        if (ComponentMountModifierPanel.isMount[i] == true)
                        {
                            if (temp.GetComponent(ComponentMountModifierPanel.components[i].GetClass()) == null)
                            {
                                temp.gameObject.AddComponent(ComponentMountModifierPanel.components[i].GetClass());
                            }
                        }
                        else
                        {
                            if (temp.GetComponent(ComponentMountModifierPanel.components[i].GetClass()) != null)
                            {
                                DestroyImmediate(temp.gameObject.GetComponent(ComponentMountModifierPanel.components[i].GetClass()));
                            }
                        }
                    }
                }
            }
        }
    }

}
