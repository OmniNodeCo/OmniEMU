using System;

namespace OmniEMU.Graphics.Video
{
    public readonly record struct Plane(IntPtr Pointer, int Length);
}
