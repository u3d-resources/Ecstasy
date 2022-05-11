#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace AV.ECS
{
    [CustomEditor(typeof(GameObjectEntity)), CanEditMultipleObjects]
    class GameObjectEntityEditor : Editor
    {
        ReorderableList list;
        SerializedProperty components;
        new GameObjectEntity target => (GameObjectEntity)base.target;
        
        static GUIStyle miniToggle;
        
        private void OnEnable()
        {
            components = serializedObject.FindProperty("components");
            var convertAll = serializedObject.FindProperty("convertAll");
            
            var minusIcon = IconContent("Toolbar Minus");
            var plusIcon = IconContent("Toolbar Plus");
            
            list = new ReorderableList(serializedObject, components)
            {
                elementHeight = 18,
                footerHeight = 0,
                displayAdd = false, displayRemove = false
            };
            list.drawFooterCallback = null;
            list.drawElementCallback += (r, index, active, focused) =>
            {
                if (index >= components.arraySize)
                    return;
                var e = components.GetArrayElementAtIndex(index);
                
                r.yMin += 1;
                r.yMax -= 1;
                
                /*
                var toggleRect = new Rect(rect) { width = 20 };
                toggleRect.y += 2;
                if (GUI.Toggle(toggleRect, true, GUIContent.none, miniToggle))
                {
                }*/
                GUI.enabled = !convertAll.boolValue;
                
                r.xMin += 20;
                r.xMax -= 30;
                EditorGUI.PropertyField(r, e, GUIContent.none);
                r.x += r.width;
                r.width = 30;
                
                GUI.enabled = true;
                
                if (GUI.Button(r, minusIcon, EditorStyles.centeredGreyMiniLabel)) 
                {
                    convertAll.boolValue = false;
                    components.DeleteArrayElementAtIndex(index);
                }
            };
            list.drawHeaderCallback += r =>
            {
                var evt = Event.current;
                var isHover = r.Contains(evt.mousePosition);
                var isDrag = isHover && DragAndDrop.visualMode == DragAndDropVisualMode.Copy;
                
                r.xMin -= 5;
                r.xMax += 5;
                
                if (isHover)
                    EditorGUI.DrawRect(r, isDrag ? new Color32(44, 93, 135, 190) : new Color32(100, 100, 100, 60));
                //new Color(0.1f, 0.5f, 0.8f, 0.4f)
                GUI.enabled = false;
                EditorGUI.IntField(new Rect(r) { width = 35 }, target.entity.id);
                GUI.enabled = true;
                
                r.x += 40;
                EditorGUI.LabelField(r, "Components", EditorStyles.miniLabel);
                
                var convertRect = new Rect(r) { x = r.width - 116, width = 80 };
                var all = convertAll.boolValue = GUI.Toggle(convertRect, convertAll.boolValue, "All", EditorStyles.toolbarButton);
                if (all)
                    EditorGUI.DrawRect(new Rect(convertRect) { yMax = 2 }, new Color(0.2f, 0.6f, 0.8f, 1) );
                
                var buttonRect = new Rect(r) {
                    x = r.width - 12, width = 20
                };
                if (GUI.Button(buttonRect, plusIcon, EditorStyles.centeredGreyMiniLabel)) 
                {
                    convertAll.boolValue = false;
                    components.InsertArrayElementAtIndex(components.arraySize);
                }
                
                if (!isHover) 
                    return;
                
                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }   
                if (evt.type == EventType.DragPerform)
                {
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is Component c) 
                        {
                            convertAll.boolValue = false;
                            
                            var size = components.arraySize;
                            components.InsertArrayElementAtIndex(size);
                            components.GetArrayElementAtIndex(size).objectReferenceValue = c;
                        }
                    }
                    evt.Use();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if (miniToggle == null)
                miniToggle = "ShurikenToggle"; 
            
            GUILayout.Space(-5);
            list.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif