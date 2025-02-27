#nullable enable
using System;
using UnityEngine;

namespace NeuroSdk.Actions
{
    [Serializable]
    public class ActionStateTransition : ScriptableObject // TODO: Custom inspector
    {
        public string startStateName = "";
        public string endStateName = "";
        public int value = 42; // Placeholder
    }
}
