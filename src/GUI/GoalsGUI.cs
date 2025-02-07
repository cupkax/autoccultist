namespace Autoccultist.GUI
{
    using System;
    using System.Linq;
    using Autoccultist.Brain;
    using Autoccultist.Config;
    using Autoccultist.GameState;
    using UnityEngine;

    /// <summary>
    /// GUI for listing and activating goals.
    /// </summary>
    public static class GoalsGUI
    {
        private static readonly Lazy<int> WindowId = new(() => GUIUtility.GetControlID(FocusType.Passive));

        private static Vector2 scrollPosition = default;

        /// <summary>
        /// Gets or sets a value indicating whether the Goals gui is being shown.
        /// </summary>
        public static bool IsShowing { get; set; }

        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        public static float Width => Mathf.Min(Screen.width, 500);

        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        public static float Height => Mathf.Min(Screen.height, 900);

        /// <summary>
        /// Draw the gui.
        /// </summary>
        public static void OnGUI()
        {
            if (!IsShowing)
            {
                return;
            }

            var offsetX = Screen.width - DiagnosticGUI.Width - Width - 10;
            var offsetY = 10;
            GUILayout.Window(WindowId.Value, new Rect(offsetX, offsetY, Width, Height), GoalsWindow, "Autoccultist Goals");
        }

        private static void GoalsWindow(int id)
        {
            GUILayout.Label("Current Goals");

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var goal in NucleusAccumbens.CurrentGoals)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Cancel", GUILayout.Width(75)))
                {
                    NucleusAccumbens.RemoveGoal(goal);
                }

                GUILayout.Label(goal.Name, GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Available Goals");

            foreach (var goal in Library.Goals)
            {
                if (goal.IsSatisfied(GameStateProvider.Current) || NucleusAccumbens.CurrentGoals.Contains(goal))
                {
                    continue;
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Activate", GUILayout.Width(75)))
                {
                    NucleusAccumbens.AddGoal(goal);
                }

                GUILayout.Label(goal.Name, GUILayout.ExpandWidth(false));

                if (goal.CanActivate(GameStateProvider.Current))
                {
                    GUILayout.Label("[CanActivate]", GUILayout.ExpandWidth(false));
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Satisfied Goals");
            foreach (var goal in Library.Goals)
            {
                if (!goal.IsSatisfied(GameStateProvider.Current))
                {
                    continue;
                }

                GUILayout.Label(goal.Name, GUILayout.ExpandWidth(false));
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                IsShowing = false;
            }
        }
    }
}
