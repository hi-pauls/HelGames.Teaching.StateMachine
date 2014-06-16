// -----------------------------------------------------------------------
// <copyright file="IState.cs" company="Paul Schulze (HelGames)">
// Copyright 2014 Paul Schulze (HelGames). All rights reserved.
// </copyright>
// <author>Paul Schulze</author>
// -----------------------------------------------------------------------
namespace HelGames.Teaching.StateMachine
{
    /// <summary>
    /// Defines the interface for a state. Classes, implementing this particular
    /// interface are valid connection points for transitions in a state machine.
    /// <para>
    /// Please note, that this interface was developed as a showcase for teaching
    /// aspiring game developers about the concept of a generic implementation of a
    /// state machines. It does account for a basic reuse pattern (which is not
    /// strictly necessary for a functional) state machine, but it is not intended
    /// for or fit for practical use. For that, the implementation of a state should
    /// at the very least be split into multiple actions, as that provides a much
    /// more flexible pattern for reuse.
    /// </para>
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets or sets the state machine, the state is connected to. This property
        /// is enforced here, because the state machine handles transitions and a
        /// transition can only happen when the <see cref="StateMachine.SendEvent" />
        /// method is called. Since a state, that does not cause a transition, is
        /// considered an end state and state machines with only end states does not
        /// make sense, states will on average require this reference. It is set in
        /// the method <see cref="StateMachine.AddState" /> to enforce consistency.
        /// </summary>
        StateMachine StateMachine { get; set; }

        /// <summary>
        /// Get the next state to be entered for the given event. This method is used
        /// by the state machine to determine the state to transition to, in case
        /// <see cref="StateMachine.SendEvent" /> is called with a specific event.
        /// Should a transition not exist, this method needs to return <c>null</c>.
        /// <para>
        /// Please note, that for a simple state machine, this approach is not strictly
        /// necessary, but it enables reuse of state types, making it a valuable
        /// implementation detail.
        /// </para>
        /// </summary>
        /// <param name="eventType">
        /// The <see cref="object"/> event, sent to the state machine.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> ID of the state, to transition to.
        /// </returns>
        object GetNextStateIdForEvent(object eventType);

        /// <summary>
        /// The state got entered and needs to be initialized or reset to perform
        /// its function properly. This will be called whenever a state is set using
        /// <see cref="StateMachine.SetState" /> or when a transition is triggered,
        /// because the state machine received the corresponding event through a call
        /// to <see cref="StateMachine.SendEvent" />. It is important to validate the
        /// existence of mandatory transitions of the state here as well, as those are
        /// defined separately.
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Update the internal data of the state. This is called once for each
        /// call to <see cref="StateMachine.Update" />, but only if the state object
        /// is the currently active state. It should be implemented for example for
        /// states, that need to continuously check the value of an object once
        /// per frame and trigger a state change, if that value has a certain
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// The state will be left, tear it down. This is called whenever a new state
        /// is set using <see cref="StateMachine.SetState" /> or when a transition is
        /// triggered using <see cref="StateMachine.SendEvent" /> with an event, that
        /// has a transition defined. Because these can potentially be triggered from
        /// outside the state and because not every transition needs to be defined for
        /// the state, this can not happen in the <see cref="IState.OnEnter" /> or the
        /// <see cref="IState.OnUpdate" /> methods.
        /// </summary>
        void OnExit();
    }
}