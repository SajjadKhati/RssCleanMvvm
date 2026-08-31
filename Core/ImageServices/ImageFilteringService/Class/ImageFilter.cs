using Core.ImageServices.ImageFilteringService.Interface;

namespace Core.ImageServices.ImageFilteringService.Class
{
    public class ImageFilter : IImageFilter
    {

        public byte[,,] ToGrayScale(byte[,,] argb8bitImage)
        {
            int height = argb8bitImage.GetLength(0);
            int width = argb8bitImage.GetLength(1);

            byte[,,] grayScaleImage = new byte[height, width, 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte alpha = argb8bitImage[y, x, 0];
                    byte red = argb8bitImage[y, x, 1];
                    byte green = argb8bitImage[y, x, 2];
                    byte blue = argb8bitImage[y, x, 3];

                    byte gray = this.CalculateGray(red, green, blue);

                    grayScaleImage[y, x, 0] = alpha;
                    grayScaleImage[y, x, 1] = gray;
                    grayScaleImage[y, x, 2] = gray;
                    grayScaleImage[y, x, 3] = gray;
                }
            }

            return grayScaleImage;
        }


        private byte CalculateGray(byte red, byte green, byte blue)
        {
            return (byte)Math.Round(0.299 * red + 0.587 * green + 0.114 * blue);
        }

    }
}
