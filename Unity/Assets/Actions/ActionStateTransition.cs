#nullable enable
using System;
using UnityEngine;

namespace NeuroSdk.Actions
{
    [Serializable]
    public class ActionStateTransition : ScriptableObject
    {
        public int startIndex;
        public int endIndex;
        public int value = 42;
    }
}
