using NUnit.Framework;
using OmniEMU.Audio.Renderer.Parameter;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer
{
    class BiquadFilterParameterTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0xC, Unsafe.SizeOf<BiquadFilterParameter>());
        }
    }
}
