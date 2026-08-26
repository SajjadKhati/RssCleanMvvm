using Shared.ImageServices.ImageUtilityService.Interface;

namespace Shared.ImageServices.ImageUtilityService.Class
{
    /// <summary>
    /// این کلاس ، به این دلیل به Shared منتقل شد چون اولا لایه ی Infra میخواد از این سرویس ، استفاده کنه .
    /// و چون لایه ی Infra ، نباید به سرویس ها یا مدل های داده هایی که در بقیه ی لایه ها پیاده سازی شدند ، وابسته بشه ، چون coupled میشه و جدا سازی و انتقال سرویس های Infra ، سخت میشه ، -
    /// پس ، به این لایه منتقل کردیم (این ، مهمترین دلیل بود) .
    /// 
    /// همچنین این سرویس و کلاس ، ممکنه در لایه های دیگه هم استفاده بشوند ، که این لایه ، بهترین لایه برای این کار هست .
    /// </summary>
    public class ImageUtility : IImageUtility
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
