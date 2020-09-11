using System;
using System.Collections;
using System.Collections.Generic;
using Snake.UI;
using UnityEditor;

[CustomEditor(typeof(ViewBase), true)]
[CanEditMultipleObjects]
public class ViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();
        SerializedProperty iterator = serializedObject.GetIterator();
        
        for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
        {
            if ("popUiModel" == iterator.propertyPath)
            {
                ViewBase v = (ViewBase) iterator.serializedObject.targetObject;
                if (v.uiType == UiType.Pop)
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                continue;
            }
            using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                EditorGUILayout.PropertyField(iterator, true);
        }
        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndChangeCheck();
    }
}
