using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    /// <summary>
    /// Base for numeric variables with optional clamping.
    /// </summary>
    public abstract class NumericVariable<T> : BaseVariable<T> where T : struct
    {
        [Header("Constraints")]
        [SerializeField] private bool useClamp;

        protected abstract T Min { get; }
        protected abstract T Max { get; }
        protected abstract T Clamp(T v, T min, T max);

        protected override T ValidateBeforeSet(T incoming)
        {
            if (!useClamp) return incoming;
            return Clamp(incoming, Min, Max);
        }
    }
}
