using GameHelper;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomPropertyDrawer.GameHelper
{
    [CustomEditor(typeof(GameHelperButton))]
    public class GameHelperButtonCustomEditor : UnityEditor.Editor
    {
        private bool _listExpanded = true;

        public override void OnInspectorGUI()
        {
            DrawStepList();
        }
        
        private void DrawStepList()
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty("_steps");
            
            _listExpanded = EditorGUILayout.Foldout(_listExpanded, "Steps");
            if (_listExpanded)
            {
                int newSize = EditorGUILayout.IntField(serializedProperty.arraySize);
                if (serializedProperty.arraySize != newSize)
                {
                    if (newSize < 0)
                        newSize = 0;
                    
                    serializedProperty.arraySize = newSize;
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                }
                
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    var layer = serializedProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(layer,GUIContent.none);
                }
            }
        }
    }
}