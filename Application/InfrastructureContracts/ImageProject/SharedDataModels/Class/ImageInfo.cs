using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;

namespace Application.InfrastructureContracts.ImageProject.SharedDataModels.Class
{
    public class ImageInfo
    {

        public int Width { get; init; }

        public int Height { get; init; }

        public int ColorDepth { get; init; }

        public ColorSpace ColorSpace { get; init; }

        public int ChannelsCount { get; init; }

        public IReadOnlyList<PixelChannel> ChannelsSequence { get; init; }




        public ImageInfo()
        {

        }


        public ImageInfo(ImageInfo imageInfo)
        {
            this.Width = imageInfo.Width;
            this.Height = imageInfo.Height;
            this.ColorDepth = imageInfo.ColorDepth;
            this.ColorSpace = imageInfo.ColorSpace;
            this.ChannelsCount = imageInfo.ChannelsCount;
            this.ChannelsSequence = imageInfo.ChannelsSequence;
        }


    }
}
