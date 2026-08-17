using NUnit.Framework;
using OmniEMU.Audio.Renderer.Common;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Audio.Renderer.Common
{
    class UpdateDataHeaderTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.AreEqual(0x40, Unsafe.SizeOf<UpdateDataHeader>());
        }
    }
}
