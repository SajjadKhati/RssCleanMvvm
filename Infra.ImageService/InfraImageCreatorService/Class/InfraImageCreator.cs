using ImageMagick;

namespace Infra.ImageService.InfraImageCreatorService.Class
{
    public class InfraImageCreator
    {

        public TColorDepth[,,] GetImageDataArray<TColorDepth>(
            int width, int height, int channelCount, IPixelCollection<TColorDepth> pixels)
            where TColorDepth : struct, IConvertible
        {
            TColorDepth[,,] data = new TColorDepth[height, width, channelCount];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = pixels.GetValue(x, y);
                    for (int channel = 0; channel < channelCount; channel++)
                    {
                        data[y, x, channel] = pixel[channel];
                    }
                }
            }

            return data;
        }

    }
}
