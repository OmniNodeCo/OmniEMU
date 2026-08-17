using OmniEMU.Graphics.OpenGL.Helper;
using System;

namespace OmniEMU.Graphics.OpenGL
{
    public interface IOpenGLContext : IDisposable
    {
        void MakeCurrent();

        bool HasContext();
    }
}
