#if UNITY_EDITOR
using UnityEditor;

namespace OmniGameTemplate.Core.Data.Variables.Editor
{
    [CustomPropertyDrawer(typeof(Vector3Reference))]
    public class Vector3ReferenceDrawer : VariableReferenceDrawerBase
    {
        protected override string ConstantFieldName => "constant";
        protected override string VariableFieldName => "variable";
    }
}
#endif
