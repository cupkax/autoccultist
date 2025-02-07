namespace Autoccultist.GameState.Impl
{
    using System;
    using System.Collections.Generic;
    using Assets.CS.TabletopUI;

    /// <summary>
    /// The backing implementation of the state of a card as derived from an element stack.
    /// </summary>
    internal class CardStateImpl : GameStateObject, ICardState
    {
        private readonly Lazy<ElementStackToken> consumed;

        private readonly string elementId;
        private readonly float lifetimeRemaining;
        private readonly bool isUnique;
        private readonly CardLocation location;
        private readonly bool isSlottable;
        private readonly IReadOnlyDictionary<string, int> aspects;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardStateImpl"/> class.
        /// </summary>
        /// <param name="sourceStack">The source stack to represent a card of.</param>
        /// <param name="location">The location of the card.</param>
        public CardStateImpl(ElementStackToken sourceStack, CardLocation location)
        {
            this.consumed = new Lazy<ElementStackToken>(() => GameAPI.TakeOneCard(sourceStack));

            this.elementId = sourceStack.EntityId;
            this.lifetimeRemaining = sourceStack.LifetimeRemaining;
            this.isUnique = sourceStack.Unique;
            this.aspects = new Dictionary<string, int>(sourceStack.GetAspects());
            this.location = location;
            this.isSlottable = location == CardLocation.Tabletop && !sourceStack.IsInAir && !sourceStack.IsBeingAnimated;
        }

        /// <inheritdoc/>
        public string ElementId
        {
            get
            {
                this.VerifyAccess();
                return this.elementId;
            }
        }

        /// <inheritdoc/>
        public float LifetimeRemaining
        {
            get
            {
                this.VerifyAccess();
                return this.lifetimeRemaining;
            }
        }

        /// <inheritdoc/>
        public bool IsUnique
        {
            get
            {
                this.VerifyAccess();
                return this.isUnique;
            }
        }

        /// <inheritdoc/>
        public CardLocation Location
        {
            get
            {
                this.VerifyAccess();
                return this.location;
            }
        }

        /// <inheritdoc/>
        public bool IsSlottable
        {
            get
            {
                this.VerifyAccess();
                return this.isSlottable;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, int> Aspects
        {
            get
            {
                this.VerifyAccess();
                return this.aspects;
            }
        }

        /// <summary>
        /// Gets an enumerable of card states for cards in the current stack.
        /// </summary>
        /// <param name="stack">The stack to derive card states from.</param>
        /// <param name="location">The location of the card stack.</param>
        /// <returns>An enumerable of card states representing cards in the given stack.</returns>
        public static IEnumerable<CardStateImpl> CardStatesFromStack(ElementStackToken stack, CardLocation location)
        {
            for (var i = 0; i < stack.Quantity; i++)
            {
                yield return new CardStateImpl(stack, location);
            }
        }

        /// <inheritdoc/>
        public ElementStackToken ToElementStack()
        {
            // FIXME: This is temporary, and can result in an existing ICardState being used up.
            //  We need to implement card consumption in place of this, so that
            //  consumed cards no longer show up for use in IGameState.TabletopCards
            this.VerifyAccess();
            return this.consumed.Value;
        }
    }
}
