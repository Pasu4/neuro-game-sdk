#nullable enable
using System;
using UnityEngine;

namespace NeuroSdk.Actions
{
    [Serializable]
    public class ActionStateTransition : ScriptableObject
    {
        public string startStateName = "";
        public string endStateName = "";
        public int value = 42;
    }
}
