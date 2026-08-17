using NUnit.Framework;
using OmniEMU.Audio.Renderer.Server.Voice;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Server
{
    class VoiceStateTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.LessOrEqual(Unsafe.SizeOf<VoiceState>(), 0x220);
        }
    }
}
