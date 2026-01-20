using System;
using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [Serializable]
    public class Vector3Reference
    {
        public bool useConstant = true;
        public Vector3 constant;
        public Vector3Variable variable;

        public Vector3 Value
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
