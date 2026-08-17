using OmniEMU.Graphics.GAL;
using OmniEMU.Memory.Range;

namespace OmniEMU.Graphics.Gpu.Memory
{
    /// <summary>
    /// GPU Index Buffer information.
    /// </summary>
    struct IndexBuffer
    {
        public MultiRange Range;
        public IndexType Type;
    }
}
