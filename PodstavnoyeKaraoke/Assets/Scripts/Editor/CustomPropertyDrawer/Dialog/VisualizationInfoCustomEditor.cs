using System;
using System.Collections.Generic;
using System.Linq;
using Dialogs.Base;
using Extensions;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomPropertyDrawer.Dialog
{
    [UnityEditor.CustomPropertyDrawer(typeof(VisualizationInfo))]
    public class VisualizationInfoCustomEditor : PropertyDrawer
    {
        private Rect _rect;
        private SerializedProperty _property;
        private TypeVisualization _typeVisualization;
        private int _countProperty = 0;
        private bool _propertiesShown;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            _countProperty = 0;
            _rect = new Rect(position);
            _rect.y -= EditorGUIUtility.singleLineHeight;
            _rect.height = EditorGUIUtility.singleLineHeight;
            _property = property;
            
            RecalculateSize();
            
            _propertiesShown = EditorGUI.Foldout(_rect, _propertiesShown,label);

            if (_propertiesShown)
            {
                EditorGUI.indentLevel = indent + 1;

                DrawType();
                DrawTweenType();
                if(_typeVisualization == TypeVisualization.Move || _typeVisualization == TypeVisualization.MoveAlpha)
                    DrawDirection();
                DrawTime();
                DrawLine();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        
        private void RecalculateSize()
        {
            _countProperty++;
            _rect.y += EditorGUIUtility.singleLineHeight + 2;
        }

        private void DrawType()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_type");
            
            var index =serializedProperty.enumValueIndex;
            var types = Enum.GetNames(typeof(TypeVisualization));
            _typeVisualization = EnumExtensions.ParseEnum<TypeVisualization>(types[index]);

            GUIContent label = new GUIContent();
            label.text = "Type";

            EditorGUI.PropertyField(_rect, serializedProperty, label);	
        }
        
        private void DrawTweenType()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_tweenType");
            GUIContent label = new GUIContent();
            label.text = "Tween type";

            EditorGUI.PropertyField(_rect, serializedProperty, label);	
        }
        
        private void DrawDirection()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_direction");

            List<string> directions = new List<string>() {"Left", "Up", "Right", "Down"};
            
            string selectDirection = "";
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
        
        private void DrawTime()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_time");
            GUIContent label = new GUIContent();
            label.text = "Time";

            EditorGUI.PropertyField(_rect, serializedProperty, label);	
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