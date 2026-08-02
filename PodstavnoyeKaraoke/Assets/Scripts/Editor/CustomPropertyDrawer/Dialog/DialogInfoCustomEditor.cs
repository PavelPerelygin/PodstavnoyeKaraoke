using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Dialogs.Base;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomPropertyDrawer.Dialog
{
    [UnityEditor.CustomPropertyDrawer(typeof(DialogInfo))]
    public class DialogInfoCustomEditor : PropertyDrawer
    {
        private Rect _rect;
        private SerializedProperty _property;
        private int _countProperty = 0;
        private bool _listExpanded = true;
        private bool _propertiesShown;
        private bool _enableFade;
        private bool _enableBlur;
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
                DrawCreatePosition();
                DrawNeedCloseByClick();
                DrawEnableFade();
                DrawHideOnSecondScreen();
                if(_enableFade)
                    DrawIntensityFade();
                DrawEnableBlur();
                if(_enableBlur)
                    DrawIntensityBlur();
                DrawBlockLayers();
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
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_typeDialog");
            GUIContent label = new GUIContent();
            label.text = "Type";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }

        private void DrawCreatePosition()
        {
            RecalculateSize();
            
            EditorGUI.LabelField(_rect,"Create position");
            
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_createPosition");
            
            var newPosition = EditorGUI.Vector3Field(_rect, "", serializedProperty.vector3Value);
            if (newPosition != serializedProperty.vector3Value)
            {
                serializedProperty.vector3Value = newPosition;
            }
        }

        private void DrawNeedCloseByClick()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_needCloseByClick");
            GUIContent label = new GUIContent();
            label.text = "Close by click";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawEnableFade()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_enableFade");
            _enableFade = serializedProperty.boolValue;
            
            GUIContent label = new GUIContent();
            label.text = "Enable fade";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawHideOnSecondScreen()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_hideOnSecondScreen");

            GUIContent label = new GUIContent();
            label.text = "Hide On Second Screen";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawIntensityFade()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_intensityFade");

            GUIContent label = new GUIContent();
            label.text = "Intensity fade";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawEnableBlur()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_enableBlur");
            _enableBlur = serializedProperty.boolValue;
            
            GUIContent label = new GUIContent();
            label.text = "Enable blur";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }
        
        private void DrawIntensityBlur()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_intensityBlur");
            GUIContent label = new GUIContent();
            label.text = "Intensity blur";

            EditorGUI.PropertyField(_rect, serializedProperty, label);
        }

        private void DrawBlockLayers()
        {
            RecalculateSize();
            
            SerializedProperty serializedProperty = _property.FindPropertyRelative("_blockLayers");
            GUIContent label = new GUIContent();
            label.text = "Block layers";

            _listExpanded = EditorGUI.Foldout(_rect, _listExpanded,"Block layers");
            if (_listExpanded)
            {
                RecalculateSize();

                serializedProperty.arraySize = EditorGUI.IntField(_rect, serializedProperty.arraySize);;
                
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    RecalculateSize();
                    
                    var layer = serializedProperty.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(_rect, layer,GUIContent.none);
                }
            }
        }
        
        private void DrawLine()
        {
            RecalculateSize();
            
            var rect = new Rect( _rect.x, _rect.y, _rect.width, 1 );
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int countLine = _countProperty;
            if (countLine > 1)
                countLine --;
            
            return (EditorGUIUtility.singleLineHeight+ 2) * (countLine);
        }
    }
}