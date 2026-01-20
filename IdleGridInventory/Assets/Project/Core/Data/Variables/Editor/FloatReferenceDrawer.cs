#if UNITY_EDITOR
using UnityEditor;

namespace OmniGameTemplate.Core.Data.Variables.Editor
{
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : VariableReferenceDrawerBase
    {
        protected override string ConstantFieldName => "constant";
        protected override string VariableFieldName => "variable";
    }
}
#endif
