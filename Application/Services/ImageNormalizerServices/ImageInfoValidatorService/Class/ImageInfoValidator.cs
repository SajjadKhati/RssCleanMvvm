using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;

namespace Application.Services.ImageNormalizerServices.ImageInfoValidatorService.Class
{
    public class ImageInfoValidator
    {

        public bool Has8BitColorDepth(int colorDepth)
        {
            if (colorDepth != 8)
                return false;
            return true;
        }


        public bool HasRgbColorSpace(ColorSpace colorSpace)
        {
            if (colorSpace == ColorSpace.RGB || colorSpace == ColorSpace.sRGB || colorSpace == ColorSpace.scRGB)
                return true;
            return false;
        }


        public bool Has4Channels(int channelsCount)
        {
            if (channelsCount != 4)
                return false;
            return true;
        }


        public bool HasSpecificChannelSequence(IEnumerable<PixelChannel> sourceChannelsSequence, 
            IEnumerable<PixelChannel> specificChannelSequence)
        {
            if (sourceChannelsSequence == null || specificChannelSequence == null)
                return false;

            return sourceChannelsSequence.SequenceEqual(specificChannelSequence);
        }


    }
}
