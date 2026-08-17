using OmniEMU.Graphics.Shader.Decoders;
using OmniEMU.Graphics.Shader.Translation;

namespace OmniEMU.Graphics.Shader.Instructions
{
    static partial class InstEmit
    {
        public static void Nop(EmitterContext context)
        {
            context.GetOp<InstNop>();

            // No operation.
        }
    }
}
