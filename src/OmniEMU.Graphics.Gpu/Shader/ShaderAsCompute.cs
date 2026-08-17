using OmniEMU.Graphics.GAL;
using OmniEMU.Graphics.Shader;
using OmniEMU.Graphics.Shader.Translation;

namespace OmniEMU.Graphics.Gpu.Shader
{
    class ShaderAsCompute
    {
        public IProgram HostProgram { get; }
        public ShaderProgramInfo Info { get; }
        public ResourceReservations Reservations { get; }

        public ShaderAsCompute(IProgram hostProgram, ShaderProgramInfo info, ResourceReservations reservations)
        {
            HostProgram = hostProgram;
            Info = info;
            Reservations = reservations;
        }
    }
}
