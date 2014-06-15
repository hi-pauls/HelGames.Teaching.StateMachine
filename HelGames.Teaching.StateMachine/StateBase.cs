// -----------------------------------------------------------------------
// <copyright file="StateBase.cs" company="HelGames Company Identifier">
// Copyright 2014 HelGames Company Identifier. All rights reserved.
// </copyright>
// <author>Paul Schulze</author>
// -----------------------------------------------------------------------
namespace HelGames.Teaching.StateMachine
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the abstract base class for a state. This is a pure convenience
    /// implementation, so states, deriving from this class, will not have to roll
    /// their own implementation of the transition handling. But of course, if
    /// a state needs to get its transition data from elsewhere, it can implement
    /// the <see cref="IState" /> interface directly.
    /// <para>
    /// Any state, deriving from this base implementation, will have to implement
    /// the abstract <see cref="StateBase.OnEnter"/>, <see cref="StateBase.OnExit"/>
    /// and <see cref="StateBase.OnUpdate"/> methods. These methods were explicitely
    /// chosen to be abstract, because the implementation is targeted at aspiring
    /// game developers and having to implement these methods (even if empty), will
    /// prompt them to think about their respective use at least shortly. In real
    /// world use, these methods may provide an empty, virtual implementation here,
    /// as it keeps state classes clean of stuff, they don't need.
    /// </para>
    /// <para>
    /// Please note, that as a showcase, this implementation it is not fit for
    /// practical use. It is missing a lot of convenience methods like looping over
    /// or removing transitions, as well as even the most basic error checking, so
    /// the actual code could be kept short and simple.
    /// </para>
    /// </summary>
    public abstract class StateBase : IState
    {
        /// <summary>
        /// Hosts the Dictionary of events, mapped to the next state. This will be used
        /// by <see cref="StateBase.GetTransitionForEvent" /> to determine, whether there
        /// is a transition away from the state exists and which state to transition to.
        /// </summary>
        private Dictionary<object, object> transitions = new Dictionary<object, object>();

        /// <summary>
        /// Gets or sets the state machine, the state is connected to. This property
        /// is enforced here, because the state machine handles transitions and a
        /// transition can only happen when the <see cref="StateMachine.SendEvent" />
        /// method is called. Since a state, that does not cause a transition, is
        /// considered an end state and state machines with only end states does not
        /// make sense, states will on average require this reference. It is set in
        /// the method <see cref="StateMachine.AddState" /> to enforce consistency.
        /// </summary>
        public StateMachine StateMachine { get; set; }

        /// <summary>
        /// Add a transition to this state. Any transition consists of the type of event,
        /// it will happen on and the state, the state machine should transition to upon
        /// receiving that event. This is a more extensible approach than just setting
        /// the new state from within the current state, as it allows reuse of states.
        /// <para>
        /// Working with the IDs of the states is also preferable to working with state
        /// objects, as a state can be completely configured without it requiring all the
        /// states to be set up, that the state may transition to. This avoids a messy
        /// initialization of the state machine, where events are added to the states
        /// separate from the state initialization.
        /// </para>
        /// <para>
        /// Please note, that there is no error checking being done here for simplicity.
        /// This implies, that the given event type may not already be registered for
        /// a transition away from this state.
        /// </para>
        /// </summary>
        /// <param name="eventType">
        /// The <see cref="object"/> event, on which the state machine should transition.
        /// </param>
        /// <param name="nextStateId">
        /// The <see cref="object"/> ID of the next state to enter, when the given event
        /// is received by the state machine.
        /// </param>
        public void AddTransition(object eventType, object nextStateId)
        {
            this.transitions.Add(eventType, nextStateId);
        }

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
        public object GetNextStateIdForEvent(object eventType)
        {
            object nextStateId;
            if (this.transitions.TryGetValue(eventType, out nextStateId))
            {
                // The transition exists, return the ID specified next state.
                return nextStateId;
            }

            return null;
        }

        /// <summary>
        /// The state got entered and needs to be initialized or reset to perform
        /// its function properly. This will be called whenever a state is set using
        /// <see cref="StateMachine.SetState" /> or when a transition is triggered,
        /// because the state machine received the corresponding event through a call
        /// to <see cref="StateMachine.SendEvent" />. It is important to validate the
        /// existence of mandatory transitions of the state here as well, as those are
        /// defined separately.
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// Update the internal data of the state. This is called once for each
        /// call to <see cref="StateMachine.Update" />, but only if the state object
        /// is the currently active state. It should be used for example for states,
        /// that need to continuously check the value of a property of an object once
        /// per frame and trigger a state change, should the property have a specific
        /// value.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// The state will be left, tear it down. This is called whenever a new state
        /// is set using <see cref="StateMachine.SetState" /> or when a transition is
        /// triggered using <see cref="StateMachine.SendEvent" /> with an event, that
        /// has a transition defined. Because these can potentially be triggered from
        /// outside the state and because not every transition needs to be defined for
        /// the state, this can not happen in the <see cref="IState.OnEnter" /> or the
        /// <see cref="IState.OnUpdate" /> methods.
        /// </summary>
        public abstract void OnExit();
    }
}