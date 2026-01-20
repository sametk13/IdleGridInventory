#if UNITY_EDITOR
using UnityEditor;

namespace OmniGameTemplate.Core.Data.Variables.Editor
{
    [CustomPropertyDrawer(typeof(BoolReference))]
    public class BoolReferenceDrawer : VariableReferenceDrawerBase
    {
        protected override string ConstantFieldName => "constant";
        protected override string VariableFieldName => "variable";
    }
}
#endif
