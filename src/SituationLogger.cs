namespace Autoccultist
{
    using System.Collections.Generic;
    using System.Text;
    using Assets.Core;
    using Assets.Core.Interfaces;
    using Assets.CS.TabletopUI;
    using Assets.TabletopUi;

    /// <summary>
    /// Utility classes for logging the status of situations.
    /// </summary>
    internal static class SituationLogger
    {
        /// <summary>
        /// Dump information about the situations to the console.
        /// </summary>
        public static void LogSituations()
        {
            LogInfo("Seeking situation tokens...");
            foreach (var situationController in GameAPI.GetAllSituations())
            {
                LogInfo("We found a situation token - " + situationController.GetTokenId());
                DumpSituationStatus(situationController);
            }

            LogInfo("...Done seeking situation tokens");
        }

        private static void DumpSituationStatus(SituationController controller)
        {
            LogInfo("- state: " + controller.SituationClock.State);
            LogInfo("- recipe id: " + controller.SituationClock.RecipeId);
            LogInfo("- time remaining: " + controller.SituationClock.TimeRemaining);

            var storedAspects = AspectsToString(controller.GetAspectsInSituation());
            LogInfo("- stored aspects: " + storedAspects);

            LogInfo("- starting slots:");
            DumpSlots(controller.situationWindow.GetStartingSlots());

            LogInfo("- ongoing slots:");
            DumpSlots(controller.situationWindow.GetOngoingSlots());

            LogInfo("- stored stacks");
            DumpElements(controller.GetStoredStacks());

            LogInfo("- output stacks");
            DumpElements(controller.GetOutputStacks());
        }

        private static void DumpSlots(IList<RecipeSlot> slots)
        {
            foreach (var slot in slots)
            {
                LogInfo("- - " + slot.GoverningSlotSpecification.Id);
                LogInfo("- - - label: " + slot.GoverningSlotSpecification.Label);
                LogInfo("- - - description: " + slot.GoverningSlotSpecification.Description);
                LogInfo("- - - greedy: " + slot.GoverningSlotSpecification.Greedy);
                LogInfo("- - - consumes: " + slot.GoverningSlotSpecification.Consumes);

                var requiredAspects = AspectsToString(slot.GoverningSlotSpecification.Required);
                LogInfo("- - - required aspects: " + requiredAspects);

                var forbiddenAspects = AspectsToString(slot.GoverningSlotSpecification.Forbidden);
                LogInfo("- - - forbidden aspects: " + forbiddenAspects);
                LogInfo("- - - primary: " + slot.IsPrimarySlot());
                LogInfo("- - - greedy: " + slot.IsGreedy);

                var content = slot.GetTokenInSlot();
                if (content != null)
                {
                    LogInfo("- - - content: " + content.EntityId);
                    var asStack = content as ElementStackToken;
                    if (asStack != null)
                    {
                        LogInfo("- - - - quantity: " + asStack.Quantity);
                        LogInfo("- - - - lifetime remaining: " + asStack.LifetimeRemaining);
                        var stackAspects = AspectsToString(asStack.GetAspects());
                        LogInfo("- - - - aspects: " + stackAspects);
                    }
                }
            }
        }

        private static void DumpElements(IEnumerable<ElementStackToken> stacks)
        {
            foreach (var stack in stacks)
            {
                LogInfo("- - " + stack.EntityId);
                LogInfo("- - - quantity: " + stack.Quantity);
                LogInfo("- - - lifetime remaining: " + stack.LifetimeRemaining);
            }
        }

        private static string AspectsToString(IAspectsDictionary aspects)
        {
            var builder = new StringBuilder();
            foreach (var aspect in aspects)
            {
                builder.AppendFormat("{0}({1}), ", aspect.Key, aspect.Value);
            }

            var str = builder.ToString();
            if (str.Length == 0)
            {
                return string.Empty;
            }

            return str.Substring(0, str.Length - 2);
        }

        private static void LogInfo(string message)
        {
            AutoccultistPlugin.Instance.LogTrace(message);
        }
    }
}
