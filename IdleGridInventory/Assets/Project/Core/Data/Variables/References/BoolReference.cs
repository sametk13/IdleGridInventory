using System;

namespace OmniGameTemplate.Core.Data.Variables
{
    [Serializable]
    public class BoolReference
    {
        public bool useConstant = true;
        public bool constant;
        public BoolVariable variable;

        public bool Value
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
