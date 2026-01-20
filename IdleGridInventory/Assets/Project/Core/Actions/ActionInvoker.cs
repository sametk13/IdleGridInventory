// File: Core/Actions/ActionInvoker.cs
using UnityEngine;

namespace OmniGameTemplate.Core.Actions
{
    /// <summary>
    /// Helper MonoBehaviour to trigger ScriptableActions from scene (UI, animation, timeline).
    /// </summary>
    public class ActionInvoker : MonoBehaviour
    {
        public ScriptableAction action;
        public ScriptableActionFloat actionFloat;

        public void InvokeAction() => action?.Execute();
        public void InvokeActionFloat(float value) => actionFloat?.Execute(value);
    }
}
