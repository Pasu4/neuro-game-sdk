#nullable enable
using UnityEditor;
using UnityEngine;
using NeuroSdk.Actions;
using System.Linq;
using System.Collections.Generic;

namespace NeuroSdk.Editor
{
    public class ActionStateMachineEditorWindow : EditorWindow
    {
        public static readonly Vector2 stateSize = new(200, 50);

        int lastSelectionID;
        ActionStateMachine? stateMachine;
        ActionState? addingTransitionState;
        List<ActionState> statesToDelete = new();
        List<ActionStateTransition> transitionsToDelete = new();

        [MenuItem("Window/Neuro SDK/Action State Machine Editor")]
        static void ShowEditor()
        {
            ActionStateMachineEditorWindow window = GetWindow<ActionStateMachineEditorWindow>();
        }

        private void Awake()
        {
            titleContent = new GUIContent("State Machine Editor");
        }

        void OnGUI()
        {
            Debug.Log(
                $"Event type: {Event.current.type}" +
                $"\nButton: {Event.current.button}"
            );

            // Check if selection has changed
            if(lastSelectionID != Selection.activeInstanceID || stateMachine != null && stateMachine.gameObject != Selection.activeGameObject)
            {
                OnSelectionChange();
            }

            if(stateMachine == null)
            {
                Debug.Log("State machine is null");
                return;
            }

            // Draw transitions
            foreach(ActionStateTransition transition in stateMachine.transitions)
            {
                DrawTransition(transition);
            }

            // Draw new transition
            if(addingTransitionState != null)
            {
                Vector2 start = addingTransitionState.EditorRect.center;
                Vector2 end = Event.current.mousePosition;
                Handles.DrawBezier(start, end, end, start, Color.white, null, 3f);
                Repaint();
            }

            // Draw windows
            BeginWindows();
            for(int i = 0; i < stateMachine.states.Count; i++)
            {
                stateMachine.states[i].EditorRect = GUI.Window(i, stateMachine.states[i].EditorRect, DrawActionState, stateMachine.states[i].stateName);
            }
            EndWindows();

            Debug.Log(
                $"Event type: {Event.current.type}" +
                $"\nButton: {Event.current.button}"
            );

            // Handle right click on background
            if(Event.current.type == EventType.MouseDown
                && Event.current.button == 1)
            {
                Vector2 pos = Event.current.mousePosition - stateSize / 2f;
                GenericMenu menu = new();
                menu.AddItem(new GUIContent("Add state"), false, () =>
                {
                    stateMachine.AddState(stateMachine.GetUniqueStateName(), false, pos);
                });
                menu.ShowAsContext();
                Event.current.Use();
            }

            // Handle deleting states and transitions
            // Not handled directly in the GUI because it would mess up the indices
            bool doRepaint = statesToDelete.Count > 0 || transitionsToDelete.Count > 0;

            foreach(ActionState state in statesToDelete)
                stateMachine.RemoveState(state);
            statesToDelete.Clear();

            foreach(ActionStateTransition transition in transitionsToDelete)
                stateMachine.RemoveTransition(transition);
            transitionsToDelete.Clear();

            if(doRepaint) Repaint();
        }

        private void OnSelectionChange()
        {
            lastSelectionID = Selection.activeInstanceID;
            Debug.Log($"Selection changed: {Selection.activeInstanceID}");
            if(Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent(out ActionStateMachine stateMachine))
            {
                Debug.Log("Found state machine");
                this.stateMachine = stateMachine;
                LoadStateMachine();
            }
            else
            {
                //Clear();
            }
        }

        /// <summary>
        /// Loads the state machine from the currently selected GameObject.
        /// </summary>
        void LoadStateMachine()
        {
            if(stateMachine == null) // Should never happen
            {
                Debug.LogError("State machine is null");
                return;
            }
            
            // Check for null states and transitions
            //if(stateMachine.states.Any(s => s == null))
            //{
            //    Debug.LogWarning("State machine contains null states, removing.");
            //    stateMachine.states.RemoveAll(s => s == null);
            //}

            //if(stateMachine.transitions.Any(t => t == null))
            //{
            //    Debug.LogWarning("State machine contains null transitions, removing.");
            //    stateMachine.transitions.RemoveAll(t => t == null);
            //}

            //if(!stateMachine.states.Any(s => s.isStartState))
            //{
            //    Debug.LogWarning("State machine has no start state, creating.");
            //    ActionState startState = CreateInstance<ActionState>();
            //    startState.stateName = "Start";
            //    startState.isStartState = true;
            //    startState.editorPos = new Vector2(0, 0);
            //    stateMachine.states.Add(startState);
            //}
        }

        void Clear()
        {
            // TODO
        }

        /// <summary>
        /// Draws the GUI for an action state.
        /// </summary>
        /// <param name="index">The index of the state in the state machine.</param>
        void DrawActionState(int index)
        {
            if(stateMachine == null) // Should never happen
            {
                Debug.LogError("State machine is null");
                return;
            }

            ActionState state = stateMachine.states[index];

            Rect clickArea = new(0, 0, state.EditorRect.width, state.EditorRect.height);

            // Handle left click
            if(clickArea.Contains(Event.current.mousePosition)
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 0)
            {
                if(addingTransitionState != null)
                {
                    stateMachine.AddTransition(addingTransitionState, state);
                    addingTransitionState = null;
                    Repaint();
                }
                else
                {
                    Debug.Log($"Clicked on {state.stateName}");
                    Selection.activeObject = state;
                }
            }

            // Handle right click
            if(clickArea.Contains(Event.current.mousePosition)
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 1)
            {
                GenericMenu menu = new();
                menu.AddItem(new GUIContent("Add transition"), false, () =>
                {
                    addingTransitionState = state;
                });
                menu.AddItem(new GUIContent("Delete state"), false, () =>
                {
                    statesToDelete.Add(state);
                });
                menu.ShowAsContext();
                Event.current.Use();
            }
            else
            {
                GUI.DragWindow();
            }
        }

        /// <summary>
        /// Draws a transition between two states.
        /// </summary>
        /// <param name="transition">The transition to draw.</param>
        void DrawTransition(ActionStateTransition transition)
        {
            if(stateMachine == null) // Should never happen
            {
                Debug.LogError("State machine is null");
                return;
            }

            ActionState? startState = stateMachine.GetState(transition.startStateName);
            ActionState? endState = stateMachine.GetState(transition.endStateName);

            if(startState == null || endState == null)
            {
                Debug.LogWarning("Transition references invalid state.");
                return;
            }

            Vector2 startPos = startState.EditorRect.center;
            Vector2 endPos = endState.EditorRect.center;
            Vector2 center = (startPos + endPos) / 2f;

            Vector2 direction = (endPos - startPos).normalized;
            Vector2 normal = new(-direction.y, direction.x);
            Vector3[] triangle = new Vector3[]
            {
                center + direction * 5,
                center - direction * 5 - normal * 5,
                center - direction * 5 + normal * 5,
            };

            //Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
            // TODO: Project line start to edge
            // TODO: Handle multiple transitions between same states
            // TODO: Handle self transitions
            Handles.DrawBezier(startPos, endPos, endPos, startPos, Color.white, null, 3f);
            Handles.DrawAAConvexPolygon(triangle);

            float clickRange = 10f;

            // Handle right click
            if(Event.current.type == EventType.MouseDown
                && Event.current.button == 1
                && (Event.current.mousePosition - center).sqrMagnitude <= (clickRange * clickRange))
            {
                GenericMenu menu = new();
                menu.AddItem(new GUIContent("Delete transition"), false, () =>
                {
                    transitionsToDelete.Add(transition);
                });
                menu.ShowAsContext();
                Event.current.Use();
            }

            // Handle left click
            if(Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && (Event.current.mousePosition - center).sqrMagnitude <= (clickRange * clickRange))
            {
                Debug.Log($"Clicked on transition from {startState.stateName} to {endState.stateName}");
                Selection.activeObject = transition;
            }
        }
    }
}