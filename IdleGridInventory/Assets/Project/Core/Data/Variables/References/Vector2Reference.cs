using System;
using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables
{
    [Serializable]
    public class Vector2Reference
    {
        public bool useConstant = true;
        public Vector2 constant;
        public Vector2Variable variable;

        public Vector2 Value
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
