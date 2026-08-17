using OmniEMU.Graphics.GAL;
using OmniEMU.Graphics.OpenGL.Image;
using System;

namespace OmniEMU.Graphics.OpenGL.Effects
{
    internal interface IScalingFilter : IDisposable
    {
        float Level { get; set; }
        void Run(
            TextureView view,
            TextureView destinationTexture,
            int width,
            int height,
            Extents2D source,
            Extents2D destination);
    }
}
