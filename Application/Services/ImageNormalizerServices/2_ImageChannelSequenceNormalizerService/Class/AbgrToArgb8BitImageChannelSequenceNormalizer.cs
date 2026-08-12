namespace Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Class
{
    public class AbgrToArgb8BitImageChannelSequenceNormalizer : BgraToArgb8BitImageChannelSequenceNormalizer
    {

        protected override byte CalculateOneChannelData(byte[,,] imageSource, int y, int x, int channel)
        {
            return (channel == 0) ? imageSource[y, x, 0] : imageSource[y, x, this.lastImageSourceIndex - (channel - 1)];
        }

    }
}
