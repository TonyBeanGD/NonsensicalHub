using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComponentMountModifier : EditorWindow {

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
        public static Object[] components;
        
    }

    private void OnGUI()
    {
        ComponentMountModifierPanel.ComponentCount= EditorGUILayout.IntField("组件数量", ComponentMountModifierPanel.ComponentCount, GUILayout.MinWidth(100f));

        if (GUILayout.Button("应用数量"))
        {
            ComponentMountModifierPanel.ApplyCount = ComponentMountModifierPanel.ComponentCount;
            ComponentMountModifierPanel.isMount = new bool[ComponentMountModifierPanel.ApplyCount];
            ComponentMountModifierPanel.components = new Object[ComponentMountModifierPanel.ApplyCount];
        }
        
        for (int i = 0; i < ComponentMountModifierPanel.ApplyCount; i++)
        {
            ComponentMountModifierPanel.components[i] = (Object)EditorGUILayout.ObjectField("组件", ComponentMountModifierPanel.components[i], typeof(Object), false, GUILayout.MinWidth(100f));
            ComponentMountModifierPanel.isMount[i] = EditorGUILayout.Toggle("是否挂载", ComponentMountModifierPanel.isMount[i], GUILayout.MinWidth(100f));
        }

        if (GUILayout.Button("应用"))
        {
            foreach (var item in ComponentMountModifierPanel.components)
            {
                var tArray = Resources.FindObjectsOfTypeAll(typeof(Transform));
                for (int i = 0; i < tArray.Length; i++)
                {
                    Transform temp = tArray[i] as Transform;

                    Undo.RecordObject(temp, temp.gameObject.name);
                    MonoBehaviour m=(item as MonoBehaviour);
                    //temp.gameObject.AddComponent <> (m);
                }
            }

           
        }
    }
}
