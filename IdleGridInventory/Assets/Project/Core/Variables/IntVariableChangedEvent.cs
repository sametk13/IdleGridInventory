// File: Core/Variables/VariableChangedEvent.cs
using UnityEngine;
using UnityEngine.Events;
using OmniGameTemplate.Core.Data.Variables;

namespace OmniGameTemplate.Core.Variables
{
    /// <summary>
    /// Bridges a Variable (SO) change into a UnityEvent you can hook in Inspector.
    /// Create typed variants as needed.
    /// </summary>
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/Value Changed Event (Int)", fileName = "ValueChanged_Int")]
    public class IntVariableChangedEvent : ScriptableObject
    {
        [Header("Observed Variable")]
        public IntVariable variable;

        [System.Serializable] public class IntEvent : UnityEvent<int,int>{} // (old, new)
        [Header("Response")]
        public IntEvent OnChanged;

        private void OnEnable()
        {
            if (variable != null)
                variable.OnValueChanged += HandleChanged;
        }

        private void OnDisable()
        {
            if (variable != null)
                variable.OnValueChanged -= HandleChanged;
        }

        private void HandleChanged(int oldVal, int newVal) => OnChanged?.Invoke(oldVal, newVal);
    }
}
