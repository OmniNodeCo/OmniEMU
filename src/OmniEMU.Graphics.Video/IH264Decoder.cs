using System;

namespace OmniEMU.Graphics.Video
{
    public interface IH264Decoder : IDecoder
    {
        bool Decode(ref H264PictureInfo pictureInfo, ISurface output, ReadOnlySpan<byte> bitstream);
    }
}
