using NUnit.Framework;
using OmniEMU.Audio.Renderer.Parameter;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer
{
    class VoiceOutStatusTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x10, Unsafe.SizeOf<VoiceOutStatus>());
        }
    }
}
