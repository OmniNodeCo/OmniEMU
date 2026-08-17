namespace OmniEMU.Memory
{
    public interface IRefCounted
    {
        void IncrementReferenceCount();
        void DecrementReferenceCount();
    }
}
