#nullable enable
using NeuroSdk.Actions;
using UnityEditor;
using UnityEngine;

namespace NeuroSdk.Editor
{
    [CustomEditor(typeof(ActionStateMachine))]
    public class ActionStateMachineEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Open editor"))
            {
                //if(EditorWindow.HasOpenInstances<ActionStateMachineEditorWindow>())
                //{
                //    EditorWindow.GetWindow<ActionStateMachineEditorWindow>().Show();
                //}
                //else
                //{
                //    EditorWindow.CreateWindow<ActionStateMachineEditorWindow>();
                //}
                EditorWindow.GetWindow<ActionStateMachineEditorWindow>().Show();
            }
        }
    }
}
