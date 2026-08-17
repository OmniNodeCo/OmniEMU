using OmniEMU.Horizon.Arp;
using OmniEMU.Horizon.Audio;
using OmniEMU.Horizon.Bcat;
using OmniEMU.Horizon.Friends;
using OmniEMU.Horizon.Hshl;
using OmniEMU.Horizon.Ins;
using OmniEMU.Horizon.Lbl;
using OmniEMU.Horizon.LogManager;
using OmniEMU.Horizon.MmNv;
using OmniEMU.Horizon.Ngc;
using OmniEMU.Horizon.Ovln;
using OmniEMU.Horizon.Prepo;
using OmniEMU.Horizon.Psc;
using OmniEMU.Horizon.Ptm;
using OmniEMU.Horizon.Sdk.Arp;
using OmniEMU.Horizon.Srepo;
using OmniEMU.Horizon.Usb;
using OmniEMU.Horizon.Wlan;
using System.Collections.Generic;
using System.Threading;

namespace OmniEMU.Horizon
{
    public class ServiceTable
    {
        private int _readyServices;
        private int _totalServices;

        private readonly ManualResetEvent _servicesReadyEvent = new(false);

        public IReader ArpReader { get; internal set; }
        public IWriter ArpWriter { get; internal set; }

        public IEnumerable<ServiceEntry> GetServices(HorizonOptions options)
        {
            List<ServiceEntry> entries = new();

            void RegisterService<T>() where T : IService
            {
                entries.Add(new ServiceEntry(T.Main, this, options));
            }

            RegisterService<ArpMain>();
            RegisterService<AudioMain>();
            RegisterService<BcatMain>();
            RegisterService<FriendsMain>();
            RegisterService<HshlMain>();
            RegisterService<HwopusMain>(); // TODO: Merge with audio once we can start multiple threads.
            RegisterService<InsMain>();
            RegisterService<LblMain>();
            RegisterService<LmMain>();
            RegisterService<MmNvMain>();
            RegisterService<NgcMain>();
            RegisterService<OvlnMain>();
            RegisterService<PrepoMain>();
            RegisterService<PscMain>();
            RegisterService<SrepoMain>();
            RegisterService<TsMain>();
            RegisterService<UsbMain>();
            RegisterService<WlanMain>();

            _totalServices = entries.Count;

            return entries;
        }

        internal void SignalServiceReady()
        {
            if (Interlocked.Increment(ref _readyServices) == _totalServices)
            {
                _servicesReadyEvent.Set();
            }
        }

        public void WaitServicesReady()
        {
            _servicesReadyEvent.WaitOne();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _servicesReadyEvent.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
