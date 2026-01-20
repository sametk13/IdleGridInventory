using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/Float Variable", fileName = "FloatVariable")]
    public class FloatVariable : NumericVariable<float>
    {
        [SerializeField] private float min = float.NegativeInfinity;
        [SerializeField] private float max = float.PositiveInfinity;

        [Header("Rounding (optional)")]
        [SerializeField] private bool useRounding;
        [SerializeField, Min(0)] private int decimals = 2;

        protected override float Min => min;
        protected override float Max => max;

        protected override float ValidateBeforeSet(float incoming)
        {
            var v = base.ValidateBeforeSet(incoming);
            if (useRounding) v = (float)System.Math.Round(v, decimals);
            return v;
        }

        protected override float Clamp(float v, float mn, float mx) => Mathf.Clamp(v, mn, mx);

        // Convenience ops
        public void Add(float amount) => SetValue(Value + amount);
        public void Multiply(float factor) => SetValue(Value * factor);
    }
}
