// -----------------------------------------------------------------------
// <copyright file="StateMachine.cs" company="HelGames Company Identifier">
// Copyright 2014 HelGames Company Identifier. All rights reserved.
// </copyright>
// <author>Paul Schulze</author>
// -----------------------------------------------------------------------
namespace HelGames.Teaching.StateMachine
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the StateMachine.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// Hosts the Dictionary of states, contained in this state machine, mapped to their ID.
        /// State-to-ID mapping was chosen here, so it is not possible to set a state, that is
        /// not contained and configured within the state machine, but to provide a fast way to
        /// get the contained state by its ID. This also makes it possible to set up states
        /// independant from each other as long as the transitions are specified with the
        /// correct IDs.
        /// </summary>
        private Dictionary<object, IState> states = new Dictionary<object, IState>();

        /// <summary>
        /// Gets the state, the state machine is currently in.
        /// </summary>
        public IState State { get; private set; }

        /// <summary>
        /// Add a state to the state machine. The ID of the state will be used to identify the
        /// state when forcing a transition using <see cref="StateMachine.SetState"/> or when
        /// a state machine event is received through <see cref="StateMachine.SendEvent"/>. It
        /// has to be unique within the state machine.
        /// <para>
        /// This method also provides some additional setup, making sure that the property
        /// <see cref="IState.StateMachine" /> is set correctly.
        /// </para>
        /// </summary>
        /// <param name="stateId">
        /// The <see cref="object"/> ID of the state to add. This ID will be used to reference
        /// the state in transitions.
        /// </param>
        /// <param name="state">
        /// The <see cref="IState"/> state instance to add for the given ID.
        /// </param>
        public void AddState(object stateId, IState state)
        {
            state.StateMachine = this;
            this.states.Add(stateId, state);
        }

        /// <summary>
        /// Set the state of the state machine using its unique ID. This will result in the
        /// current state being left and the new state being entered, ignoring any transitions
        /// (or lack thereof) between the current state and the state being set.
        /// <para>
        /// Use of this method is generally not advised, as it ignores the chain of transitions,
        /// designed and implemented for the configured state machine. However, any state machine
        /// has to use this method at least once, during initialization, to set the initial state,
        /// the state machine will be in before <see cref="StateMachine.Update"/> is called
        /// for the very first time. Otherwise, <see cref="StateMachine.State"/> will be
        /// <c>null</c>, resulting in a <see cref="NullReferenceException"/> during the update.
        /// It can also be used to reset the state machine to its start-state, for example on
        /// level transitions, avoiding a complete tear-down and re-setup of an existing state
        /// machine. For transitioning to a state, use <see cref="StateMachine.SendEvent"/>, from
        /// inside or outside the state machine. This will result in the transition only taking
        /// place at appropriate times, where the current state defines a transition for the
        /// event and thereby ensuring that the behavior of the state machine remains consistent.
        /// </para>
        /// <para>
        /// Please note, that the state will not change, if the state with the given ID is not
        /// defined in the dictionary of states. This also applies to a state ID of <c>null</c>.
        /// Once any state is set, this implementation does not provide a way to un-set it.
        /// </para>
        /// </summary>
        /// <param name="stateId">
        /// The <see cref="object"/> ID of the state to enter. The state with the given ID needs
        /// to exist in the context of the state machine.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Is thrown, if there is no state defined for the given ID.
        /// </exception>
        public void SetState(object stateId)
        {
            IState state;
            if (! this.states.TryGetValue(stateId, out state))
            {
                // The state is not defined, don't transition to it. Tell someone.
                throw new ArgumentException(string.Format("Invalid state ID: {0}", stateId));
            }

            if (this.State != null)
            {
                this.State.OnExit();
            }

            this.State = state;
            this.State.OnEnter();
        }

        /// <summary>
        /// Send an event to the state machine, potentially resulting in a transition, if there
        /// is one defined for the state.
        /// <para>
        /// This method can be used from inside or outside the state machine. It is the preferred
        /// way to change states over <see cref="StateMachine.SetState"/>, as the transition will
        /// only happen, if it is defined for the state, the state machine is currently in. This
        /// ensures, that the behavior of the state machine remains consistent to its design.
        /// </para>
        /// </summary>
        /// <param name="eventType">
        /// The <see cref="object"/> event to send to the state machine. The state machine will
        /// transition to a new state upon receiving this event, provided there is a transition
        /// defined for the event.
        /// </param>
        public void SendEvent(object eventType)
        {
            object nextStateId = this.State.GetNextStateIdForEvent(eventType);
            if (nextStateId != null)
            {
                this.SetState(nextStateId);
            }
        }

        /// <summary>
        /// Update the state machine. This will result in only the currently set state being
        /// updated and thereby limiting the amount of code, running at any given update call.
        /// <para>
        /// This method should only be called after a the desired initial state was set using
        /// <see cref="StateMachine.SetState"/>. If no state was set when calling this method,
        /// a <see cref="NullReferenceException"/> will be thrown.
        /// </para>
        /// </summary>
        public void Update()
        {
            this.State.OnUpdate();
        }
    }
}