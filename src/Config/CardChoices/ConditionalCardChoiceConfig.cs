namespace Autoccultist.Config.CardChoices
{
    using System.Collections.Generic;
    using Autoccultist.Config.Conditions;
    using Autoccultist.GameState;

    /// <summary>
    /// Selects a card from its choice only if a condition is met.
    /// </summary>
    public class ConditionalCardChoiceConfig : ISlottableCardChoiceConfig, IConfigObject
    {
        /// <summary>
        /// Gets or sets the condition to gate this choice.
        /// </summary>
        public IGameStateConditionConfig Condition { get; set; }

        /// <summary>
        /// Gets or sets the card choice to use if the condition is met.
        /// </summary>
        public ISlottableCardChoiceConfig Choice { get; set; }

        /// <inheritdoc/>
        public ICardState ChooseCard(IEnumerable<ICardState> cards)
        {
            var state = GameStateProvider.Current;
            if (!this.Condition.IsConditionMet(state))
            {
                return null;
            }

            return this.Choice.ChooseCard(cards);
        }
    }
}
