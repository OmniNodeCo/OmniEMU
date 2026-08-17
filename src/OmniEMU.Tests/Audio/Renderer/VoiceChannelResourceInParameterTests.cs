using NUnit.Framework;
using OmniEMU.Audio.Renderer.Parameter;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer
{
    class VoiceChannelResourceInParameterTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x70, Unsafe.SizeOf<VoiceChannelResourceInParameter>());
        }
    }
}
