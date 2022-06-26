using UnityEditor;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class EditorStyleExtensions
    {
        #region DrawHeader

        public static void DrawHeader(string title)
        {
            GUILayout.Space(5);
            GUILayout.Label(title, EditorStyles.boldLabel);
        }

        public static void DrawHeader(string title, string description)
        {
            GUILayout.Space(5);
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.Label(description, EditorStyles.miniLabel);
        }

        public static void DrawHeader(string title, string description, string url)
        {
            GUILayout.Space(5);
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.Label(description, EditorStyles.miniLabel);
            GUILayout.Label(url, EditorStyles.linkLabel);
        }

        #endregion

        #region Container

        public static void BeginContainer(string title, string description, int space = 5)
        {
            GUILayout.Space(space);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            DrawHeader(title, description);
        }

        public static void EndContainer(int space = 5)
        {
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }

        #endregion

        #region EditorIcon

        public static Texture2D PlayBtnIcon() =>
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "PlayButton On@2x" : "PlayButton@2x")
                .image as Texture2D;

        public static Texture2D StopBtnIcon() =>
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_PreMatQuad@2x" : "PreMatQuad@2x")
                .image as Texture2D;

        public static readonly Texture2D InspectorWindow =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin
                ? "d_UnityEditor.InspectorWindow@2x"
                : "UnityEditor.InspectorWindow@2x").image as Texture2D;

        public static readonly Texture2D PlayTexture =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_PlayButton On" : "PlayButton On")
                .image as Texture2D;

        public static readonly Texture2D PauseTexture =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_PauseButton On" : "PauseButton On")
                .image as Texture2D;

        public static readonly Texture2D PrevKeyTexture =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Animation.PrevKey" : "Animation.PrevKey")
                .image as Texture2D;

        public static readonly Texture2D NextKeyTexture =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Animation.NextKey" : "Animation.NextKey")
                .image as Texture2D;

        public static readonly Texture2D MenuTexture =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d__Menu@2x" : "_Menu@2x").image as Texture2D;

        public static readonly Texture2D Menu2Texture = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin
            ? "d_UnityEditor.SceneHierarchyWindow@2x"
            : "UnityEditor.SceneHierarchyWindow@2x").image as Texture2D;

        public static readonly Texture2D VisibilityOnIcon =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_VisibilityOn" : "VisibilityOn")
                .image as Texture2D;

        public static readonly Texture2D VisibilityOnX2Icon =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_ViewToolOrbit On@2x" : "ViewToolOrbit On@2x")
                .image as Texture2D;

        public static readonly Texture2D HideIcon =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_VisibilityOff" : "VisibilityOff")
                .image as Texture2D;

        public static readonly Texture2D AnimationAddEvent =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Animation.AddEvent" : "Animation.AddEvent")
                .image as Texture2D;

        public static readonly Texture2D AnimationEvent =
            EditorGUIUtility
                .IconContent(EditorGUIUtility.isProSkin ? "d_Animation.EventMarker" : "Animation.EventMarker")
                .image as Texture2D;

        public static readonly Texture2D LockTexture =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_InspectorLock" : "InspectorLock")
                .image as Texture2D;

        public static readonly Texture2D SpeedScale =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_SpeedScale" : "SpeedScale").image as Texture2D;

        public static readonly Texture2D SearchIcon =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Search Icon" : "Search Icon")
                .image as Texture2D;


        public static readonly Texture2D EditorWindowTexture = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin
            ? "d_RectMask2D Icon"
            : "RectMask2D Icon").image as Texture2D;
        // public static readonly Texture2D EditorWindowTexture = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin
        // ? "d_UnityEditor.Timeline.TimelineWindow@2x"
        // : "UnityEditor.Timeline.TimelineWindow@2x").image as Texture2D;

        public static readonly Texture2D SaveIcon =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_SaveAs@2x" : "SaveAs@2x").image as Texture2D;

        public static readonly Texture2D LoopBtn =
            EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_playLoopOff" : "playLoopOff")
                .image as Texture2D;

        #endregion

        #region Drawers

        public static void DrawHorizontalLine(Color color, float height = 1f, float width = 0f)
        {
            var rect = GUILayoutUtility.GetRect(0f, height, width, height);
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawVerticalLine(Color color, float width = 1f, float height = 0f)
        {
            var rect = GUILayoutUtility.GetRect(width, width, height, height);
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawSeparator(float height = 1f, float width = 0f)
        {
            var rect = GUILayoutUtility.GetRect(0f, height, width, height);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, 1f), Color.black);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, 1f, rect.height), Color.black);
            EditorGUI.DrawRect(new Rect(rect.xMax, rect.yMin, -1f, rect.height), Color.black);
        }


        #endregion

        public static GUIStyle GetStyle(string styleName)
            => EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle(styleName);
    }
}