using NUnit.Framework;
using OmniEMU.Audio.Renderer.Common;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Common
{
    class WaveBufferTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x30, Unsafe.SizeOf<WaveBuffer>());
        }
    }
}
