using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/Vector2 Variable", fileName = "Vector2Variable")]
    public class Vector2Variable : BaseVariable<Vector2>
    {
        public void Add(Vector2 delta) => SetValue(Value + delta);
        public void Scale(Vector2 scale) => SetValue(Vector2.Scale(Value, scale));
        public void SetX(float x) => SetValue(new Vector2(x, Value.y));
        public void SetY(float y) => SetValue(new Vector2(Value.x, y));
    }
}
