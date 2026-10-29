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


        /// <summary>
        /// تبدیل IPixelCollection به byte[,,] با ترتیب ARGB.
        /// ترتیب کانال توسط Magick.NET (native C++) اعمال می‌شود — بدون حلقه‌ی managed.
        /// </summary>
        public byte[,,] GetArgb8BitImageDataArray(int width, int height, int channelCount, IPixelCollection<byte> pixels)
        {
            /// ToByteArray یک متد روی IPixelCollection است که آرایه‌ی تخت byte[] برمی‌گرداند.
            /// با PixelMapping.ARGB، Magick.NET ترتیب کانال را در حین این تبدیل (native) اعمال می‌کند.
            /// (اگر PixelMapping داده نشود، ترتیب پیش‌فرض خود Magick.NET — مثلاً RGBA — استفاده می‌شود.)
            byte[] flatArgbPixels = pixels.ToByteArray(PixelMapping.ARGB);
            /// آرایه‌ی سه‌بعدی خالی با ابعاد [height, width, channelCount] می‌سازیم.
            /// در حافظه، این آرایه به‌صورت پیوسته (row-major) ذخیره می‌شود.
            byte[,,] imageData = new byte[height, width, channelCount];
            /// Buffer.BlockCopy یک کپی خام حافظه (native memcpy) انجام می‌دهد — بدون حلقه‌ی managed.
            /// چون byte[] و byte[,,] هر دو در حافظه پیوسته‌اند، این کپی دقیقاً همان layout را حفظ می‌کند.
            /// پارامترها: (source, srcOffset, destination, dstOffset, count)
            Buffer.BlockCopy(flatArgbPixels, 0, imageData, 0, flatArgbPixels.Length);
            /// آرایه‌ی پرشده با ترتیب ARGB برمی‌گردانیم.
            return imageData;
        }

    }
}
