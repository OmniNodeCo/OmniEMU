using System;

namespace OmniEMU.Ava.UI.ViewModels
{
    public class AboutWindowViewModel : BaseModel, IDisposable
    {
        public string Version { get; } = Program.Version;

        public string Creator => "OmniNodeCo";

        public string ProjectDescription =>
            "A native, cross-platform Nintendo Switch emulator focused on accuracy, performance, and a polished desktop experience.";

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
