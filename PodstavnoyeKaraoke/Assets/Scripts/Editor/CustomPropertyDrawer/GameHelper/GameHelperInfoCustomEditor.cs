using System.Collections.Generic;
using GameHelper;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomPropertyDrawer.GameHelper
{
    [UnityEditor.CustomPropertyDrawer(typeof(GameHelperInfo))]
    public class GameHelperInfoCustomEditor : PropertyDrawer
    {
        private Rect _rect;
        private SerializedProperty _property;
        private int _countProperty = 0;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            _rect = new Rect(position);
            _rect.y -= EditorGUIUtility.singleLineHeight;
            _rect.height = EditorGUIUtility.singleLineHeight;
            _property = property;
            _countProperty = 0;

            DrawTarget();
            DrawHeaderId();
            DrawMessageId();
            DrawDirection();
            DrawLine();
            
            EditorGUI.EndProperty();
        }
        private void RecalculateSize()
        {
            _countProperty++;
            _rect.y += EditorGUIUtility.singleLineHeight + 2;
        }

        private void DrawTarget()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_target");
            GUIContent label = new GUIContent();
            label.text = "Target";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawHeaderId()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_headerId");
            GUIContent label = new GUIContent();
            label.text = "Header ID";
            
            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawMessageId()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_messageId");
            GUIContent label = new GUIContent();
            label.text = "Message ID";
            
            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawDirection()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_direction");
            
            serializedProperty.vector2Value = Vector2.down;
            serializedProperty.serializedObject.ApplyModifiedProperties();
            List<string> directions = new List<string>() {"Left", "Up", "Right", "Down"};
            
            string selectDirection = "Down";
            Vector2 direction = serializedProperty.vector2Value;
            if (direction == Vector2.left)
                selectDirection = "Left";
            else if (direction == Vector2.up)
                selectDirection = "Up";
            else if (direction == Vector2.right)
                selectDirection = "Right";
            else if (direction == Vector2.down)
                selectDirection = "Down";

            int index = directions.IndexOf(selectDirection);
            
            index = EditorGUI.Popup(_rect, "Direction", index, directions.ToArray());

            string newDirection = directions[index];

            if (newDirection != selectDirection)
            {
                if(newDirection == "Left")
                    serializedProperty.vector2Value = Vector2.left;
                else if(newDirection == "Up")
                    serializedProperty.vector2Value = Vector2.up;
                else if(newDirection == "Right")
                    serializedProperty.vector2Value = Vector2.right;
                else if(newDirection == "Down")
                    serializedProperty.vector2Value = Vector2.down;
            }
        }
        
        private void DrawLine()
        {
            RecalculateSize();
            
            var rectLine = new Rect( _rect.x, _rect.y, _rect.width, 1 );
            EditorGUI.DrawRect(rectLine, new Color(0.5f, 0.5f, 0.5f, 1));
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int countLine = _countProperty;
            if (countLine > 1)
                countLine --;
            
            return (EditorGUIUtility.singleLineHeight + 4) * (countLine);
        }
    }
}