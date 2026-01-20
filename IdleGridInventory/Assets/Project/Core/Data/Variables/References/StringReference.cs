using System;

namespace OmniGameTemplate.Core.Data.Variables
{
    [Serializable]
    public class StringReference
    {
        public bool useConstant = true;
        public string constant;
        public StringVariable variable;

        public string Value
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
