#if UNITY_EDITOR
using UnityEditor;

namespace OmniGameTemplate.Core.Data.Variables.Editor
{
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : VariableReferenceDrawerBase
    {
        protected override string ConstantFieldName => "constant";
        protected override string VariableFieldName => "variable";
    }
}
#endif
