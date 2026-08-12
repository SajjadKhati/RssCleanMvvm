namespace Core.ImageServices.ImageCreatorService.Class
{
    public abstract class ImageCreator<TColorDepth> where TColorDepth : struct, IConvertible
    {

        protected TColorDepth[,,] CreateImage(TColorDepth[,,] imageSource, int imageTargetChannelCount)
        {
            int height = imageSource.GetLength(0);
            int width = imageSource.GetLength(1);

            TColorDepth[,,] targetImageData = new TColorDepth[height, width, imageTargetChannelCount];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int channel = 0; channel < imageTargetChannelCount; channel++)
                    {
                        targetImageData[y, x, channel] = this.CalculateOneChannelData(imageSource, y, x, channel);
                    }
                }
            }
            return targetImageData;
        }


        protected abstract TColorDepth CalculateOneChannelData(TColorDepth[,,] imageSource, int y, int x, int channel);

    }
}
