using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Variables/Bool Variable", fileName = "BoolVariable")]
    public class BoolVariable : BaseVariable<bool>
    {
        public void Toggle() => SetValue(!Value);
    }
}
