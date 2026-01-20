using System;

namespace OmniGameTemplate.Core.Data.Variables
{
    public interface IVariable<T>
    {
        T Value { get; set; }
        event Action<T, T> OnValueChanged; // (oldValue, newValue)
        void SetValue(T newValue);
        void SetValueSilently(T newValue);
        void ResetToDefault();
    }
}
