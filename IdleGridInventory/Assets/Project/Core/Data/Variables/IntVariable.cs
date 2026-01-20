using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/Int Variable", fileName = "IntVariable")]
    public class IntVariable : NumericVariable<int>
    {
        [SerializeField] private int min = int.MinValue;
        [SerializeField] private int max = int.MaxValue;

        protected override int Min => min;
        protected override int Max => max;

        protected override int Clamp(int v, int mn, int mx) => Mathf.Clamp(v, mn, mx);

        public void Add(int amount) => SetValue(Value + amount);
        public void Multiply(int factor) => SetValue(Value * factor);
    }
}
