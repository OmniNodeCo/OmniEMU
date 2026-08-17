using OmniEMU.Common.Memory;

namespace OmniEMU.Graphics.Nvdec.Vp9.Types
{
    internal struct BModeInfo
    {
        public PredictionMode Mode;
        public Array2<Mv> Mv; // First, second inter predictor motion vectors
    }
}
