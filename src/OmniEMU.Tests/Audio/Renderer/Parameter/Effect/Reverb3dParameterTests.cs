using NUnit.Framework;
using OmniEMU.Audio.Renderer.Parameter.Effect;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Parameter.Effect
{
    class Reverb3dParameterTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x49, Unsafe.SizeOf<Reverb3dParameter>());
        }
    }
}
