using System;
using UnityEngine;
using UnityEngine.Events;

namespace OmniGameTemplate.Core.Data.Variables
{
    /// <summary>
    /// Generic ScriptableObject-backed variable with change notifications.
    /// </summary>
    public abstract class BaseVariable<T> : ScriptableObject, IVariable<T>
    {
        [Header("Value")]
        [SerializeField] protected T defaultValue;
        [SerializeField] protected T value;

        public event Action<T, T> OnValueChanged;
        public UnityEvent<T> OnValueChangedUnity;

        public virtual T Value
        {
            get => value;
            set => SetValue(value);
        }

        protected virtual void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            value = defaultValue;
        }

        protected virtual T ValidateBeforeSet(T incoming) => incoming;

        public virtual void SetValue(T newValue)
        {
            var validated = ValidateBeforeSet(newValue);
            if (Equals(validated, value)) return;

            var old = value;
            value = validated;

            OnValueChanged?.Invoke(old, value);
            OnValueChangedUnity?.Invoke(value);
        }

        public virtual void SetValueSilently(T newValue)
        {
            value = ValidateBeforeSet(newValue);
        }

        public virtual void ResetToDefault() => SetValue(defaultValue);

        public void Apply(Func<T, T> mutator) => SetValue(mutator(Value));
    }
}
