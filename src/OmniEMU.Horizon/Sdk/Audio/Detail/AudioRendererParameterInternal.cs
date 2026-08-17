using OmniEMU.Audio.Renderer.Parameter;

namespace OmniEMU.Horizon.Sdk.Audio.Detail
{
    struct AudioRendererParameterInternal
    {
        public AudioRendererConfiguration Configuration;

        public AudioRendererParameterInternal(AudioRendererConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
