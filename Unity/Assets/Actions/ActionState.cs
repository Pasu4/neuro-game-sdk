#nullable enable
using UnityEngine;
using System;

namespace NeuroSdk.Actions
{
    [Serializable]
    public class ActionState : ScriptableObject // TODO: Custom inspector
    {
        public Rect EditorRect
        {
            get
            {
                return new Rect(editorPos, new Vector2(200, 100));
            }
            set
            {
                editorPos = value.position;
            }
        }
        public Vector2 editorPos = Vector2.zero;
        public bool isStartState = false;
        public string stateName = "";
    }
}
