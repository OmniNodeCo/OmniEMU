using OmniEMU.HLE.HOS.Tamper.Operations;

namespace OmniEMU.HLE.HOS.Tamper.CodeEmitters
{
    /// <summary>
    /// Code type 0xFF1 resumes the current process.
    /// </summary>
    class ResumeProcess
    {
        // FF1?????
        public static void Emit(byte[] instruction, CompilationContext context)
        {
            context.CurrentOperations.Add(new OpProcCtrl(context.Process, false));
        }
    }
}
