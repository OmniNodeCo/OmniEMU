using OmniEMU.Graphics.GAL.Multithreading.Commands.Sampler;
using OmniEMU.Graphics.GAL.Multithreading.Model;

namespace OmniEMU.Graphics.GAL.Multithreading.Resources
{
    class ThreadedSampler : ISampler
    {
        private readonly ThreadedRenderer _renderer;
        public ISampler Base;

        public ThreadedSampler(ThreadedRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Dispose()
        {
            _renderer.New<SamplerDisposeCommand>().Set(new TableRef<ThreadedSampler>(_renderer, this));
            _renderer.QueueCommand();
        }
    }
}
