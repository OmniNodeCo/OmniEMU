namespace OmniEMU.Horizon.Common
{
    public interface IThreadContext
    {
        bool Running { get; }

        ulong TlsAddress { get; }

        ulong GetX(int index);
    }
}
