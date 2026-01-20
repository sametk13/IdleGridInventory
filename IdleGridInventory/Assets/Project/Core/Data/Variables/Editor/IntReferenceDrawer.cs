#if UNITY_EDITOR
using UnityEditor;

namespace OmniGameTemplate.Core.Data.Variables.Editor
{
    [CustomPropertyDrawer(typeof(IntReference))]
    public class IntReferenceDrawer : VariableReferenceDrawerBase
    {
        protected override string ConstantFieldName => "constant";
        protected override string VariableFieldName => "variable";
    }
}
#endif
