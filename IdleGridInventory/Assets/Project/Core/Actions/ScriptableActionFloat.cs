// File: Core/Actions/ScriptableActionFloat.cs
using UnityEngine;
using UnityEngine.Events;

namespace OmniGameTemplate.Core.Actions
{
    /// <summary>
    /// Action with a float payload. Create more typed variants as needed (int, string, Vector3).
    /// </summary>
    [CreateAssetMenu(menuName = "OmniGameTemplate/Actions/Action (Float)", fileName = "Action_Float")]
    public class ScriptableActionFloat : ScriptableObject
    {
        [System.Serializable] public class FloatEvent : UnityEvent<float> {}

        [Header("Invoked when Execute(value) is called")]
        public FloatEvent OnExecute;

        public void Execute(float value) => OnExecute?.Invoke(value);
    }
}
