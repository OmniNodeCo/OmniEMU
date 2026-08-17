using OmniEMU.Graphics.Device;
using OmniEMU.Graphics.Host1x;
using OmniEMU.Graphics.Nvdec;
using OmniEMU.Graphics.Vic;
using System;
using GpuContext = OmniEMU.Graphics.Gpu.GpuContext;

namespace OmniEMU.HLE.HOS.Services.Nv
{
    class Host1xContext : IDisposable
    {
        public DeviceMemoryManager Smmu { get; }
        public NvMemoryAllocator MemoryAllocator { get; }
        public Host1xDevice Host1x { get; }

        public Host1xContext(GpuContext gpu, ulong pid)
        {
            MemoryAllocator = new NvMemoryAllocator();
            Host1x = new Host1xDevice(gpu.Synchronization);
            Smmu = gpu.CreateDeviceMemoryManager(pid);
            var nvdec = new NvdecDevice(Smmu);
            var vic = new VicDevice(Smmu);
            Host1x.RegisterDevice(ClassId.Nvdec, nvdec);
            Host1x.RegisterDevice(ClassId.Vic, vic);
        }

        public void Dispose()
        {
            Host1x.Dispose();
        }
    }
}
