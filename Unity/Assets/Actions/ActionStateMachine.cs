#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeuroSdk.Actions
{
    [ExecuteInEditMode]
    public class ActionStateMachine : MonoBehaviour
    {
        //[HideInInspector]
        public List<ActionState> states = new();
        //[HideInInspector]
        public List<ActionStateTransition> transitions = new();
        //[HideInInspector]
        public ActionState? currentState;

        public bool active = false;

        void Awake()
        {
            if(!Application.isPlaying)
            {
                //if(states.Count == 0)
                //{
                //    ActionState startState = ScriptableObject.CreateInstance<ActionState>();
                //    startState.stateName = "Start";
                //    startState.isStartState = true;
                //    startState.editorPos = new Vector2(0, 0);
                //    states.Add(startState);
                //}
            }
        }

        void Start()
        {

        }

        void Update()
        {
            if(active)
            {
                if(currentState == null)
                    currentState = states.FirstOrDefault(s => s.isStartState);

                if(currentState != null)
                {
                    // TODO
                }
                else
                {
                    Debug.LogError("State machine has no start state.");
                }
            }
        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="name">The name of the state. Should be a unique identifier.</param>
        /// <param name="isStartState">Whether the state is the start state. Only one state should be the start state.</param>
        /// <returns>The state that was added.</returns>
        /// <param name="editorPosition">The position of the state in the state machine editor.</param>
        public ActionState AddState(string name, bool isStartState, Vector2 editorPosition)
        {
            ActionState newState = ScriptableObject.CreateInstance<ActionState>();
            newState.stateName = name;
            newState.editorPos = editorPosition;
            newState.isStartState = isStartState;
            states.Add(newState);
            return newState;
        }

        /// <inheritdoc cref="AddState(string, bool, Vector2)"/>
        public ActionState AddState(string name, bool isStartState = false) => AddState(name, isStartState, Vector2.zero);

        /// <summary>
        /// Gets a state from the state machine by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The state with that name, or <see langword="null"/> if the state machine has no state with that name.</returns>
        public ActionState? GetState(string name) => states.FirstOrDefault(s => s.stateName == name);

        /// <summary>
        /// Removes a state and all its transitions from the state machine, if it exists.
        /// </summary>
        /// <param name="state">The state to remove.</param>
        /// <returns>The number of transitions removed, or <c>-1</c> if no state was removed.</returns>
        public int RemoveState(ActionState state)
        {
            int count = transitions.RemoveAll(t => t.startStateName == state.stateName || t.endStateName == state.stateName);
            if(states.Remove(state))
                return count;
            return -1;
        }

        /// <param name="stateName">The name of the state to remove.</param>
        /// <inheritdoc cref="RemoveState(ActionState)"/>
        public int RemoveState(string stateName)
        {
            ActionState? state = GetState(stateName);
            if(state == null)
                return -1;
            return RemoveState(state);
        }

        /// <summary>
        /// Adds a state transition to the state machine.
        /// </summary>
        /// <param name="startStateName">The name of the start state of the transition.</param>
        /// <param name="endStateName">The name of the end state of the transition.</param>
        /// <returns>The transition that was added.</returns>
        /// <exception cref="ArgumentException">One or both of the states are not in the state machine.</exception>
        public ActionStateTransition AddTransition(string startStateName, string endStateName)
        {
            if(!states.Any(s => s.stateName == startStateName) || !states.Any(s => s.stateName == endStateName))
                throw new ArgumentException("One or both of the states are not in the state machine.");

            ActionStateTransition newTransition = ScriptableObject.CreateInstance<ActionStateTransition>();
            newTransition.startStateName = startStateName;
            newTransition.endStateName = endStateName;
            transitions.Add(newTransition);
            return newTransition;
        }

        /// <param name="startState">The start state of the transition.</param>
        /// <param name="endState">The end state of the transition.</param>
        /// <inheritdoc cref="AddTransition(string, string)"/>
        public ActionStateTransition AddTransition(ActionState startState, ActionState endState)
            => AddTransition(startState.stateName, endState.stateName);

        /// <summary>
        /// Gets all transitions between two states.
        /// </summary>
        /// <param name="startState">The start state of the transitions.</param>
        /// <param name="endState">The end state of the transitions.</param>
        /// <returns>All transitions from <paramref name="startState"/> to <paramref name="endState"/>.</returns>
        public IEnumerable<ActionStateTransition> GetTransitions(ActionState startState, ActionState endState)
            => GetTransitions(startState.stateName, endState.stateName);

        /// <param name="startStateName">The name of the start state of the transitions.</param>
        /// <param name="endStateName">The name of the end state of the transitions.</param>
        /// <returns>All transitions from the state named <paramref name="startStateName"/> to the state named <paramref name="endStateName"/>.</returns>
        /// <inheritdoc cref="GetTransitions(ActionState, ActionState)"/>
        public IEnumerable<ActionStateTransition> GetTransitions(string startStateName, string endStateName)
        {
            return transitions.Where(t => t.startStateName == startStateName && t.endStateName == endStateName);
        }

        /// <summary>
        /// Gets all incoming transitions to a state.
        /// </summary>
        /// <param name="state">The state to get incoming transitions for.</param>
        /// <returns>All incoming transitions to the state.</returns>
        public IEnumerable<ActionStateTransition> GetIncomingTransitions(ActionState state)
        {
            return transitions.Where(t => t.endStateName == state.stateName);
        }

        /// <param name="stateName">The name of the state to get incoming transitions for.</param>
        /// <inheritdoc cref="GetIncomingTransitions(ActionState)"/>
        public IEnumerable<ActionStateTransition> GetIncomingTransitions(string stateName)
        {
            return transitions.Where(t => t.endStateName == stateName);
        }

        /// <summary>
        /// Removes a transitions from the state machine, if it exists.
        /// </summary>
        /// <param name="transition">The transition to remove.</param>
        /// <returns><see langword="true"/> if a transition was removed, <see langword="false"/> otherwise.</returns>
        public bool RemoveTransition(ActionStateTransition transition)
        {
            return transitions.Remove(transition);
        }
    }
}