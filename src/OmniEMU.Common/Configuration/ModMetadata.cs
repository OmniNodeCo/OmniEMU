using System.Collections.Generic;

namespace OmniEMU.Common.Configuration
{
    public struct ModMetadata
    {
        public List<Mod> Mods { get; set; }

        public ModMetadata()
        {
            Mods = new List<Mod>();
        }
    }
}
