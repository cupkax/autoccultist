namespace Autoccultist.Config
{
    using System.Collections.Generic;
    using System.Linq;
    using Autoccultist;
    using Autoccultist.Config.Conditions;
    using Autoccultist.GameState;
    using Autoccultist.Yaml;
    using YamlDotNet.Core;

    /// <summary>
    /// Represents a choice of a card based on various attributes.
    /// </summary>
    public class CardChooserConfig : ICardChooser, IConfigObject, IAfterYamlDeserialization
    {
        /// <summary>
        /// Specify whether the card choice should go for the oldest or youngest card it can find.
        /// </summary>
        public enum CardAgeSelection
        {
            /// <summary>
            /// The choice should choose the card with the least lifetime remaining.
            /// </summary>
            Oldest,

            /// <summary>
            /// The choice should choose the card with the most lifetime remaining.
            /// </summary>
            Youngest,
        }

        /// <summary>
        /// Gets or sets the element id of the card to choose.
        /// If left empty, the element id will not be factored into the card choice.
        /// </summary>
        public string ElementId { get; set; }

        /// <summary>
        /// Gets or sets the location the card must be at to be chosen.
        /// </summary>
        public CardLocation? Location { get; set; }

        /// <summary>
        /// Gets or sets the list of element ids from which to choose a card.
        /// </summary>
        public List<string> AllowedElementIds { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of aspect names to degrees to filter the cards by.
        /// If set, a matching card must have all of the specified aspects of at least the given degree.
        /// </summary>
        public Dictionary<string, ValueCondition> Aspects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the card must or must not be unique.
        /// </summary>
        public bool? Unique { get; set; }

        /// <summary>
        /// Gets or sets a list of aspects forbidden to be on the chosen card.
        /// Mainly used when specifying matching aspects.
        /// </summary>
        public List<string> ForbiddenAspects { get; set; }

        /// <summary>
        /// Gets or sets a list of elements forbidden from being matched.
        /// Mainly used when specifying matching aspects.
        /// </summary>
        public List<string> ForbiddenElementIds { get; set; }

        /// <summary>
        /// Gets or sets the age bias by which to choose time-limited cards.
        /// </summary>
        public CardAgeSelection? AgeBias { get; set; }

        /// <summary>
        /// Gets or sets a condition for the decay timer.
        /// </summary>
        public ValueCondition LifetimeRemaining { get; set; }

        /// <summary>
        /// Choose a card from the given card states based on this filter's rules.
        /// </summary>
        /// <param name="cards">The cards to choose from.</param>
        /// <returns>The chosen card, or <c>null</c> if none were chosen.</returns>
        public ICardState ChooseCard(IEnumerable<ICardState> cards)
        {
            // Once again, the lack of covariance in IReadOnlyDictionary comes back to bite us
            var aspectsAsCondition = this.Aspects?.ToDictionary(entry => entry.Key, entry => entry.Value as IValueCondition);

            var candidates =
                from card in cards
                where this.ElementId == null || card.ElementId == this.ElementId
                where this.Location == null || card.Location == this.Location
                where this.LifetimeRemaining?.IsConditionMet(card.LifetimeRemaining) != false
                where this.AllowedElementIds?.Contains(card.ElementId) != false
                where this.ForbiddenElementIds?.Contains(card.ElementId) != true
                where aspectsAsCondition == null || card.Aspects.HasAspects(aspectsAsCondition)
                where this.ForbiddenAspects?.Intersect(card.Aspects.Keys).Any() != true
                where !this.Unique.HasValue || card.IsUnique == this.Unique.Value
                where this.AdditionalFilter(card)
                select card;

            // Sort for age bias.
            if (this.AgeBias.HasValue)
            {
                if (this.AgeBias == CardAgeSelection.Oldest)
                {
                    candidates = candidates.OrderBy(card => card.LifetimeRemaining);
                }
                else if (this.AgeBias == CardAgeSelection.Youngest)
                {
                    candidates = candidates.OrderByDescending(card => card.LifetimeRemaining);
                }
            }
            else
            {
                candidates = candidates.OrderBy(card => card.Aspects.GetWeight());
            }

            return candidates.FirstOrDefault();
        }

        /// <inheritdoc/>
        void IAfterYamlDeserialization.AfterDeserialized(Mark start, Mark end)
        {
            if (string.IsNullOrEmpty(this.ElementId) && (this.Aspects == null || this.Aspects.Count == 0) && (this.AllowedElementIds == null || this.AllowedElementIds.Count == 0))
            {
                throw new InvalidConfigException("Card choice must have an elementId, allowedElementIds, or aspects.");
            }
        }

        /// <summary>
        /// Performs additional filtering on a chosen card.
        /// </summary>
        /// <param name="card">The card to filter.</param>
        /// <returns><c>true</c> if this card should be selected, or <c>false</c> if it should not.</returns>
        protected virtual bool AdditionalFilter(ICardState card)
        {
            return true;
        }
    }
}
