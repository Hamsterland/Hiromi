using System;

namespace Hiromi.Services.Attributes
{
    public class HelpDisplay : Attribute
    {
        public HelpDisplayOptions Option { get; }

        public HelpDisplay(HelpDisplayOptions option)
        {
            Option = option;
        }
    }
}