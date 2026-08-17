using System;

namespace OmniEMU.HLE.Exceptions
{
    public class TamperCompilationException : Exception
    {
        public TamperCompilationException(string message) : base(message) { }
    }
}
