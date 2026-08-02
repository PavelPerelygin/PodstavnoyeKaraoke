using UnityEditor;
using UnityEngine;

namespace Tools.Editor.OpenAssetsHistory
{
    public class OpenAssetsHistoryWindow : EditorWindow
    {
        private Vector3 _scrollPosition = Vector3.zero;
        private AssetInfo _needOpenAssetInfo;

        [MenuItem("Tools/Open Assets History")]
        public static void ShowWindow()
        {
            Texture texture = EditorGUIUtility.IconContent("CloudConnect").image;
            var windows = GetWindow(typeof(OpenAssetsHistoryWindow));
            windows.autoRepaintOnSceneChange = true;
            windows.titleContent = new GUIContent("History",texture);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawHistory();
            DrawClearButton();
            GUILayout.EndVertical();

            if (_needOpenAssetInfo != null)
            {
                OpenAssetInHistory(_needOpenAssetInfo);
                _needOpenAssetInfo = null;
            }
        }

        private void DrawClearButton()
        {
            if (GUILayout.Button("Clear history", GUILayout.ExpandWidth(true)))
            {
                OpenAssetsHistory.ClearHistory();
            }
        }

        private void DrawHistory()
        {
            GUILayout.BeginVertical("scrollView");
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition,false,false,GUIStyle.none, GUI.skin.verticalScrollbar);
            for (int i = 0; i < OpenAssetsHistory._history._assets.Count; i++)
            {
                DrawOfferItem(OpenAssetsHistory._history._assets[i]);
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        private void DrawOfferItem(AssetInfo info)
        {
            GUILayout.BeginHorizontal();
            
            Color textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            GUIStyleState styleState = new GUIStyleState()
            {
                background = Texture2D.blackTexture,
                textColor = textColor
            };

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(10,0,0,0),
                border = new RectOffset(0,0,0,0),
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                
                normal = styleState,
                hover = styleState,
                active = styleState,
                focused = styleState
                
            };

            Texture icon = EditorGUIUtility.IconContent(info._iconName).image;
            GUIContent guiContent = new GUIContent(info._name,icon);
            
            if (GUILayout.Button(guiContent, style,GUILayout.Height(15)))
            {
                _needOpenAssetInfo = info;
            }
            GUILayout.EndHorizontal();
        }
        
        
        private void OpenAssetInHistory(AssetInfo info)
        {
            OpenAssetsHistory.OpenAssetInHistory(info);
            
            Repaint();
        }
    }
}