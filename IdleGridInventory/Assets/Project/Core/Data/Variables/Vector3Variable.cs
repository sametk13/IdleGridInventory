using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/Vector3 Variable", fileName = "Vector3Variable")]
    public class Vector3Variable : BaseVariable<Vector3>
    {
        public void Add(Vector3 delta) => SetValue(Value + delta);
        public void Scale(Vector3 scale) => SetValue(Vector3.Scale(Value, scale));
        public void SetX(float x) => SetValue(new Vector3(x, Value.y, Value.z));
        public void SetY(float y) => SetValue(new Vector3(Value.x, y, Value.z));
        public void SetZ(float z) => SetValue(new Vector3(Value.x, Value.y, z));
        public void Normalize() { var v = Value; v.Normalize(); SetValue(v); }
        public void ClampMagnitude(float maxMag)
        {
            if (maxMag <= 0f) { SetValue(Vector3.zero); return; }
            SetValue(Vector3.ClampMagnitude(Value, maxMag));
        }
    }
}
