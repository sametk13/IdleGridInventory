using System;
using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [Serializable]
    public class FloatReference
    {
        public bool useConstant = true;
        public float constant;
        public FloatVariable variable;

        public float Value
        {
            get => useConstant || variable == null ? constant : variable.Value;
            set
            {
                if (useConstant || variable == null) constant = value;
                else variable.Value = value;
            }
        }
    }
}
