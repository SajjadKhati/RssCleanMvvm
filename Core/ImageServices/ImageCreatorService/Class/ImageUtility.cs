namespace Core.ImageServices.ImageCreatorService.Class
{
    public class ImageUtility
    {

        /// <summary>
        ///  تخت کردن آرایه سه‌بعدی به آرایه یک‌ بعدی .
        ///  یعنی یک داده ی تصویر را میگیره و اطلاعاتش را فقط در یک آرایه ی یک بعدی میریزه .
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public byte[] FlattenImageData(byte[,,] imageData, int height, int width, int channels)
        {
            var flat = new byte[width * height * channels];
            int index = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    for (int c = 0; c < channels; c++)
                        flat[index++] = imageData[y, x, c];
            return flat;
        }

    }
}
