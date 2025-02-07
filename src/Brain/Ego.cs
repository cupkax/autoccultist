namespace Autoccultist.Brain
{
    using System;
    using System.Linq;
    using Autoccultist.GameState;

    /// <summary>
    /// Tracks execution of a single motivation.
    /// </summary>
    public static class Ego
    {
        static Ego()
        {
            NucleusAccumbens.OnGoalCompleted += OnGoalCompleted;
        }

        /// <summary>
        /// Raised when the current motivation is completed.
        /// </summary>
        public static event EventHandler<MotivationCompletedEventArgs> OnMotivationCompleted;

        /// <summary>
        /// Gets a value indicating whether the task driver is running.
        /// </summary>
        public static bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the current motivation.
        /// </summary>
        public static IMotivation CurrentMotivation { get; private set; }

        /// <summary>
        /// Sets the task list for the TaskDriver to execute.
        /// </summary>
        /// <param name="motivation">The motivation to run.</param>
        public static void SetMotivation(IMotivation motivation)
        {
            TryStopMotivation();

            CurrentMotivation = motivation;

            if (IsRunning)
            {
                TryStartMotivation();
            }
        }

        /// <summary>
        /// Starts the TaskDriver executing its tasks.
        /// </summary>
        public static void Start()
        {
            IsRunning = true;
            TryStartMotivation();
        }

        /// <summary>
        /// Stops the TaskDriver from executing its tasks.
        /// </summary>
        public static void Stop()
        {
            IsRunning = false;
            TryStopMotivation();
        }

        private static void TryStartMotivation()
        {
            if (!IsRunning || CurrentMotivation == null)
            {
                return;
            }

            var state = GameStateProvider.Current;

            foreach (var goal in CurrentMotivation.PrimaryGoals)
            {
                if (!goal.IsSatisfied(state))
                {
                    NucleusAccumbens.AddGoal(goal);
                }
            }

            foreach (var goal in CurrentMotivation.SupportingGoals)
            {
                if (!goal.IsSatisfied(state))
                {
                    NucleusAccumbens.AddGoal(goal);
                }
            }

            CheckAllGoalsCompleted();
        }

        private static void TryStopMotivation()
        {
            if (CurrentMotivation == null)
            {
                return;
            }

            foreach (var goal in CurrentMotivation.PrimaryGoals)
            {
                NucleusAccumbens.RemoveGoal(goal);
            }

            foreach (var goal in CurrentMotivation.SupportingGoals)
            {
                NucleusAccumbens.RemoveGoal(goal);
            }
        }

        private static void CheckAllGoalsCompleted()
        {
            if (CurrentMotivation.PrimaryGoals.Any(x => NucleusAccumbens.CurrentGoals.Contains(x)))
            {
                return;
            }

            AutoccultistPlugin.Instance.LogTrace($"Ego: All primary goals of motivation \"{CurrentMotivation.Name}\" are now complete.");
            TryStopMotivation();
            var completedMotivation = CurrentMotivation;
            CurrentMotivation = null;
            OnMotivationCompleted?.Invoke(null, new MotivationCompletedEventArgs(completedMotivation));
        }

        private static void OnGoalCompleted(object sender, GoalCompletedEventArgs e)
        {
            if (!IsRunning || CurrentMotivation == null)
            {
                return;
            }

            CheckAllGoalsCompleted();
        }
    }
}
