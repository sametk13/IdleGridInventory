using System;

namespace OmniGameTemplate.Core.Data.Variables
{
    [Serializable]
    public class IntReference
    {
        public bool useConstant = true;
        public int constant;
        public IntVariable variable;

        public int Value
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
