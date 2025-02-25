#nullable enable
using UnityEditor;
using UnityEngine;
using NeuroSdk.Actions;
using System.Linq;

namespace NeuroSdk.Editor
{
    public class ActionStateMachineEditorWindow : EditorWindow
    {
        public static readonly Vector2 stateSize = new(200, 50);

        int lastSelectionID;
        ActionStateMachine? stateMachine;
        ActionState? addingTransitionState;

        [MenuItem("Window/Neuro SDK/Action State Machine Editor")]
        static void ShowEditor()
        {
            GetWindow<ActionStateMachineEditorWindow>();
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
                GenericMenu menu = new();
                menu.AddItem(new GUIContent("Add state"), false, () =>
                {
                    ActionState newState = CreateInstance<ActionState>();
                    newState.editorPos = Event.current.mousePosition - stateSize / 2f;
                    newState.stateName = "State " + stateMachine.states.Count;
                    stateMachine.states.Add(newState);
                });
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private void OnSelectionChange()
        {
            lastSelectionID = Selection.activeInstanceID;
            Debug.Log($"Selection changed: {Selection.activeInstanceID}");
            if(Selection.activeGameObject.TryGetComponent(out ActionStateMachine stateMachine))
            {
                this.stateMachine = stateMachine;
                LoadStateMachine();
            }
            else
            {
                //Clear();
            }
        }

        void LoadStateMachine()
        {
            if(stateMachine == null) // Should never happen
            {
                Debug.LogError("State machine is null");
                return;
            }
            
            // Check for null states and transitions
            if(stateMachine.states.Any(s => s == null))
            {
                Debug.LogWarning("State machine contains null states, removing.");
                stateMachine.states.RemoveAll(s => s == null);
            }

            if(stateMachine.transitions.Any(t => t == null))
            {
                Debug.LogWarning("State machine contains null transitions, removing.");
                stateMachine.transitions.RemoveAll(t => t == null);
            }

            if(!stateMachine.states.Any(s => s.isStartState))
            {
                Debug.LogWarning("State machine has no start state, creating.");
                ActionState startState = CreateInstance<ActionState>();
                startState.stateName = "Start";
                startState.isStartState = true;
                startState.editorPos = new Vector2(0, 0);
                stateMachine.states.Add(startState);
            }
        }

        void Clear()
        {
            // TODO
        }

        void DrawActionState(int id)
        {
            if(stateMachine == null) // Should never happen
            {
                Debug.LogError("State machine is null");
                return;
            }

            ActionState state = stateMachine.states[id];

            Rect clickArea = new(0, 0, state.EditorRect.width, state.EditorRect.height);

            // Handle left click
            if(clickArea.Contains(Event.current.mousePosition)
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 0)
            {
                if(addingTransitionState != null)
                {
                    ActionStateTransition newTransition = CreateInstance<ActionStateTransition>();
                    newTransition.startIndex = stateMachine.states.IndexOf(addingTransitionState);
                    newTransition.endIndex = stateMachine.states.IndexOf(state);
                    stateMachine.transitions.Add(newTransition);
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
                menu.ShowAsContext();
                Event.current.Use();
            }
            else
            {
                GUI.DragWindow();
            }
        }

        void DrawTransition(ActionStateTransition transition)
        {
            if(stateMachine == null) // Should never happen
            {
                Debug.LogError("State machine is null");
                return;
            }

            Vector3 startPos = stateMachine.states[transition.startIndex].EditorRect.center;
            Vector3 endPos = stateMachine.states[transition.endIndex].EditorRect.center;

            //Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
            // TODO: Project line start to edge
            Handles.DrawBezier(startPos, endPos, endPos, startPos, Color.white, null, 3f);
        }
    }
}