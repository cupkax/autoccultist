namespace Autoccultist.Actor.Actions
{
    /// <summary>
    /// An action that closes a situation window.
    /// </summary>
    public class CloseSituationAction : IAutoccultistAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseSituationAction"/> class.
        /// </summary>
        /// <param name="situationId">The id of the situation to close.</param>
        public CloseSituationAction(string situationId)
        {
            this.SituationId = situationId;
        }

        /// <summary>
        /// Gets the situation id of the situation to close the window for.
        /// </summary>
        public string SituationId { get; }

        /// <inheritdoc/>
        public void Execute()
        {
            if (GameAPI.IsInMansus)
            {
                return;
            }

            var situation = GameAPI.GetSituation(this.SituationId);
            if (situation == null)
            {
                return;
            }

            situation.CloseWindow();
        }
    }
}
