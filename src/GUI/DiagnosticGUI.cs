namespace Autoccultist.GUI
{
    using System;
    using System.Linq;
    using Autoccultist.Brain;
    using Autoccultist.Config;
    using UnityEngine;

    /// <summary>
    /// GUI for general diagnostics.
    /// </summary>
    public static class DiagnosticGUI
    {
        private static readonly Lazy<int> WindowId = new Lazy<int>(() => GUIUtility.GetControlID(FocusType.Passive));

        /// <summary>
        /// Gets or sets a value indicating whether the window is being shown.
        /// </summary>
        public static bool IsShowing { get; set; }

        public static float Width => Mathf.Min(Screen.width, 350);

        public static float Height => Mathf.Min(Screen.height, 500);

        public static float OffsetX => Screen.width - Width - 10;

        /// <summary>
        /// Draw gui.
        /// </summary>
        public static void OnGUI()
        {
            if (!IsShowing)
            {
                return;
            }

            var height = Mathf.Min(Screen.height, 500);
            GUILayout.Window(WindowId.Value, new Rect(OffsetX, 10, Width, Height), DiagnosticsWindow, "Autoccultist Diagnostics");
        }

        private static void DiagnosticsWindow(int id)
        {
            if (Library.ParseErrors.Count > 0)
            {
                GUILayout.Label($"{Library.ParseErrors.Count} parse errors.");
                if (GUILayout.Button("Toggle Errors"))
                {
                    ParseErrorsGUI.IsShowing = !ParseErrorsGUI.IsShowing;
                }
            }

            var mechHeart = GUILayout.Toggle(MechanicalHeart.IsRunning, "Mechanical Heart");
            if (mechHeart != MechanicalHeart.IsRunning)
            {
                if (mechHeart)
                {
                    MechanicalHeart.Start();
                }
                else
                {
                    MechanicalHeart.Stop();
                }
            }

            if (GUILayout.Button("Step Heart"))
            {
                MechanicalHeart.Step();
            }

            var taskRunner = GUILayout.Toggle(TaskDriver.IsRunning, "Task Driver");
            if (taskRunner != TaskDriver.IsRunning)
            {
                if (taskRunner)
                {
                    TaskDriver.Start();
                }
                else
                {
                    TaskDriver.Stop();
                }
            }

            if (GUILayout.Button("Reload All Configs"))
            {
                AutoccultistPlugin.Instance.ReloadTasks();
            }

            GUILayout.Label("Current Goals:\n" + string.Join("\n", GoalDriver.CurrentGoals.Select(x => x.Name)));

            if (GUILayout.Button("Toggle Goals Menu"))
            {
                GoalsGUI.IsShowing = !GoalsGUI.IsShowing;
            }

            GUILayout.Label("Current Orchestrations:\n" + string.Join("\n", SituationOrchestrator.CurrentOrchestrations.Select(entry => $"{entry.Key}: {entry.Value}")));
        }
    }
}
