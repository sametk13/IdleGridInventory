using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/String Variable", fileName = "StringVariable")]
    public class StringVariable : BaseVariable<string>
    {
        [Header("Validation (optional)")]
        [SerializeField] private int maxLength = 0; // 0 = unlimited
        [SerializeField] private bool trimWhitespace;

        protected override string ValidateBeforeSet(string incoming)
        {
            var v = incoming ?? string.Empty;
            if (trimWhitespace) v = v.Trim();
            if (maxLength > 0 && v.Length > maxLength) v = v.Substring(0, maxLength);
            return v;
        }

        public void Append(string text) => SetValue((Value ?? string.Empty) + (text ?? string.Empty));
        public void Clear() => ResetToDefault();
    }
}
