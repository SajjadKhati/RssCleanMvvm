using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.SharedProject.ImageFileExtensionResolver.Interface;

namespace Infra.SharedService.ImageFileExtensionResolverService.Class
{
    public class ImageFileExtensionResolver : IImageFileExtensionResolver
    {

        public string GetFileExtension(MagickFormat magickFormat)
        {
            return magickFormat switch
            {
                MagickFormat.Png or MagickFormat.Png00 or MagickFormat.Png8 or MagickFormat.Png24 or MagickFormat.Png32
                    or MagickFormat.Png48 or MagickFormat.Png64 => "png",
                MagickFormat.Jpeg or MagickFormat.Jpg => "jpg",
                MagickFormat.Bmp => "bmp",
                MagickFormat.Tif or MagickFormat.Tiff or MagickFormat.Tiff64 => "tiff",
                _ => throw new FormatException("فرمت نباید در حالت ناشناخته باشد .")
            };
        }

    }
}
