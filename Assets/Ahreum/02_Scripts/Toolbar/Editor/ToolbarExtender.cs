#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private static readonly int m_toolCount;
        private static GUIStyle m_commandStyle;

        public static readonly List<Action> LeftToolbarGUI = new();
        public static readonly List<Action> RightToolbarGUI = new();

        static ToolbarExtender()
        {
            var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");

#if UNITY_2019_1_OR_NEWER
            const string fieldName = "k_ToolCount";
#else
			string fieldName = "s_ShownToolIcons";
#endif

            var toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_3_OR_NEWER
            m_toolCount = toolIcons != null ? (int)toolIcons.GetValue(null) : 8;
#elif UNITY_2019_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
#else
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
#endif

            ToolbarCallback.OnToolbarGUI = OnGUI;
            ToolbarCallback.OnToolbarGUILeft = GUILeft;
            ToolbarCallback.OnToolbarGUIRight = GUIRight;
        }

#if UNITY_2019_3_OR_NEWER
        private const float space = 8;
#else
		public const float space = 10;
#endif
        public const float largeSpace = 20;
        private const float buttonWidth = 32;
        private const float dropdownWidth = 80;
#if UNITY_2019_1_OR_NEWER
        private const float playPauseStopWidth = 140;
#else
		public const float playPauseStopWidth = 100;
#endif

        private static void OnGUI()
        {
            // Create two containers, left and right
            // Screen is whole toolbar

            m_commandStyle ??= new GUIStyle("CommandLeft");

            var screenWidth = EditorGUIUtility.currentViewWidth;

            // Following calculations match code reflected from Toolbar.OldOnGUI()
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - playPauseStopWidth) / 2);

            var leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += space; // Spacing left
            leftRect.xMin += buttonWidth * m_toolCount; // Tool buttons
#if UNITY_2019_3_OR_NEWER
            leftRect.xMin += space; // Spacing between tools and pivot
#else
			leftRect.xMin += largeSpace; // Spacing between tools and pivot
#endif
            leftRect.xMin += 64 * 2; // Pivot buttons
            leftRect.xMax = playButtonsPosition;

            var rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += m_commandStyle.fixedWidth * 3; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= space; // Spacing right
            rightRect.xMax -= dropdownWidth; // Layout
            rightRect.xMax -= space; // Spacing between layout and layers
            rightRect.xMax -= dropdownWidth; // Layers
#if UNITY_2019_3_OR_NEWER
            rightRect.xMax -= space; // Spacing between layers and account
#else
			rightRect.xMax -= largeSpace; // Spacing between layers and account
#endif
            rightRect.xMax -= dropdownWidth; // Account
            rightRect.xMax -= space; // Spacing between account and cloud
            rightRect.xMax -= buttonWidth; // Cloud
            rightRect.xMax -= space; // Spacing between cloud and collab
            rightRect.xMax -= 78; // Colab

            // Add spacing around existing controls
            leftRect.xMin += space;
            leftRect.xMax -= space;
            rightRect.xMin += space;
            rightRect.xMax -= space;

            // Add top and bottom margins
#if UNITY_2019_3_OR_NEWER
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;
#else
			leftRect.y = 5;
			leftRect.height = 24;
			rightRect.y = 5;
			rightRect.height = 24;
#endif

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in LeftToolbarGUI) handler();

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in RightToolbarGUI) handler();

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        private static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in LeftToolbarGUI) handler();
            GUILayout.EndHorizontal();
        }

        private static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in RightToolbarGUI) handler();
            GUILayout.EndHorizontal();
        }
    }
}
#endif