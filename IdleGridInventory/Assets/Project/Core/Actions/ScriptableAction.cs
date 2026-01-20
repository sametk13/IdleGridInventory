// File: Core/Actions/ScriptableAction.cs
using UnityEngine;
using UnityEngine.Events;

namespace OmniGameTemplate.Core.Actions
{
    /// <summary>
    /// Parameterless action that can be invoked from code or Inspector.
    /// </summary>
    [CreateAssetMenu(menuName = "OmniGameTemplate/Actions/Action", fileName = "Action")]
    public class ScriptableAction : ScriptableObject
    {
        [Header("Invoked when Execute() is called")]
        public UnityEvent OnExecute;

        public virtual void Execute() => OnExecute?.Invoke();
    }
}
