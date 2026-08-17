using NUnit.Framework;
using OmniEMU.Audio.Renderer.Parameter;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Parameter
{
    class SinkOutStatusTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x20, Unsafe.SizeOf<SinkOutStatus>());
        }
    }
}
