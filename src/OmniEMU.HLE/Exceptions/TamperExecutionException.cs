using System;

namespace OmniEMU.HLE.Exceptions
{
    public class TamperExecutionException : Exception
    {
        public TamperExecutionException(string message) : base(message) { }
    }
}
