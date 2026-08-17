using NUnit.Framework;
using OmniEMU.Audio.Renderer.Parameter.Performance;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Parameter
{
    class PerformanceInParameterTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x10, Unsafe.SizeOf<PerformanceInParameter>());
        }
    }
}
