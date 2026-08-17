using NUnit.Framework;
using OmniEMU.Audio.Renderer.Common;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Common
{
    class VoiceUpdateStateTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.LessOrEqual(Unsafe.SizeOf<VoiceUpdateState>(), 0x100);
        }
    }
}
