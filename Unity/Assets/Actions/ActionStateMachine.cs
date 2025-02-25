#nullable enable
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

        void Awake()
        {
            if(!Application.isPlaying)
            {
                if(states.Count == 0)
                {
                    ActionState startState = ScriptableObject.CreateInstance<ActionState>();
                    startState.stateName = "Start";
                    startState.isStartState = true;
                    startState.editorPos = new Vector2(0, 0);
                    states.Add(startState);
                }
            }

            currentState = states.FirstOrDefault(s => s.isStartState);
            if(currentState == null)
            {
                Debug.LogError("No start state found");
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}