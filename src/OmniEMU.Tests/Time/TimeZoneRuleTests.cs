using NUnit.Framework;
using OmniEMU.HLE.HOS.Services.Time.TimeZone;
using System.Runtime.CompilerServices;

namespace OmniEMU.Tests.Time
{
    internal class TimeZoneRuleTests
    {
        class EffectInfoParameterTests
        {
            [Test]
            public void EnsureTypeSize()
            {
                Assert.AreEqual(0x4000, Unsafe.SizeOf<TimeZoneRule>());
            }
        }
    }
}
