using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using System.ComponentModel;

namespace Infra.ImageService.MappingService.Class
{
    public class ImageFileSettingsMemberMapper
    {

        public ImageInfoMemberMapper ImageInfoMemberMapper { get; set; }


        /////////////////////////////////////////////////////////


        public ImageFileSettingsMemberMapper(ImageInfoMemberMapper imageInfoMemberMapper)
        {
            this.ImageInfoMemberMapper = imageInfoMemberMapper;
        }


        /////////////////////////////////////////////////////////


        public ImageMagick.StorageType ToMagickStorageType(StorageType applicationStorageType)
        {
            return applicationStorageType switch
            {
                StorageType.Undefined => ImageMagick.StorageType.Undefined,
                StorageType.Byte => ImageMagick.StorageType.Char,
                StorageType.Short => ImageMagick.StorageType.Short,
                StorageType.Int32 => ImageMagick.StorageType.Int32,
                StorageType.Int64 => ImageMagick.StorageType.Int64,
                StorageType.Float => ImageMagick.StorageType.Float,
                StorageType.Double => ImageMagick.StorageType.Double,
            };
        }


        public StorageType ToApplicationStorageType(ImageMagick.StorageType magicStorageType)
        {
            return magicStorageType switch
            {
                ImageMagick.StorageType.Undefined => StorageType.Undefined,
                ImageMagick.StorageType.Char => StorageType.Byte,
                ImageMagick.StorageType.Short => StorageType.Short,
                ImageMagick.StorageType.Int32 => StorageType.Int32,
                ImageMagick.StorageType.Int64 => StorageType.Int64,
                ImageMagick.StorageType.Float => StorageType.Float,
                ImageMagick.StorageType.Double => StorageType.Double,
                _ => throw new InvalidEnumArgumentException($"مقدار جاری ، در نوع {typeof(StorageType)} پشتیبانی نمیشود .")
            };
        }


        ///////////////////////////////////////////////////////////////////////////////////


        public ImageMagick.PixelMapping ToMagickPixelMapping(PixelMapping applicationPixelMapping)
        {
            return applicationPixelMapping switch
            {
                PixelMapping.ABGR => ImageMagick.PixelMapping.ABGR,
                PixelMapping.ARGB => ImageMagick.PixelMapping.ARGB,
                PixelMapping.BGR => ImageMagick.PixelMapping.BGR,
                PixelMapping.BGRA => ImageMagick.PixelMapping.BGRA,
                PixelMapping.RGB => ImageMagick.PixelMapping.RGB,
                PixelMapping.RGBA => ImageMagick.PixelMapping.RGBA,
            };
        }


        public PixelMapping ToApplicationPixelMapping(ImageMagick.PixelMapping magicPixelMapping)
        {
            return magicPixelMapping switch
            {
                ImageMagick.PixelMapping.ABGR => PixelMapping.ABGR,
                ImageMagick.PixelMapping.ARGB => PixelMapping.ARGB,
                ImageMagick.PixelMapping.BGR => PixelMapping.BGR,
                ImageMagick.PixelMapping.BGRA => PixelMapping.BGRA,
                ImageMagick.PixelMapping.RGB => PixelMapping.RGB,
                ImageMagick.PixelMapping.RGBA => PixelMapping.RGBA,
                _ => throw new InvalidEnumArgumentException($"مقدار جاری ، در نوع {typeof(PixelMapping)} پشتیبانی نمیشود .")
            };
        }


        ///////////////////////////////////////////////////////////////////////////////////


        public ImageMagick.AlphaOption ToMagickAlphaOption(AlphaOption applicationAlphaOption)
        {
            return applicationAlphaOption switch
            {
                AlphaOption.Undefined => ImageMagick.AlphaOption.Undefined,
                AlphaOption.Off => ImageMagick.AlphaOption.Off,
                AlphaOption.On => ImageMagick.AlphaOption.On,
            };
        }


        public AlphaOption ToApplicationAlphaOption(ImageMagick.AlphaOption magicAlphaOption)
        {
            return magicAlphaOption switch
            {
                ImageMagick.AlphaOption.Undefined => AlphaOption.Undefined,
                ImageMagick.AlphaOption.Off => AlphaOption.Off,
                ImageMagick.AlphaOption.On => AlphaOption.On,
                _ => throw new InvalidEnumArgumentException($"مقدار جاری ، در نوع {typeof(AlphaOption)} پشتیبانی نمیشود .")
            };
        }


        ///////////////////////////////////////////////////////////////////////////////////



        public ImageMagick.MagickFormat ToMagickMagickFormat(MagickFormat applicationMagickFormat)
        {
            return applicationMagickFormat switch
            {
                MagickFormat.Unknown => ImageMagick.MagickFormat.Unknown,
                MagickFormat.Bmp => ImageMagick.MagickFormat.Bmp,
                MagickFormat.Jpeg => ImageMagick.MagickFormat.Jpeg,
                MagickFormat.Jpg => ImageMagick.MagickFormat.Jpg,
                MagickFormat.Png => ImageMagick.MagickFormat.Png,
                MagickFormat.Png00 => ImageMagick.MagickFormat.Png00,
                MagickFormat.Png24 => ImageMagick.MagickFormat.Png24,
                MagickFormat.Png32 => ImageMagick.MagickFormat.Png32,
                MagickFormat.Png48 => ImageMagick.MagickFormat.Png48,
                MagickFormat.Png64 => ImageMagick.MagickFormat.Png64,
                MagickFormat.Png8 => ImageMagick.MagickFormat.Png8,
                MagickFormat.Tif => ImageMagick.MagickFormat.Tif,
                MagickFormat.Tiff => ImageMagick.MagickFormat.Tiff,
                MagickFormat.Tiff64 => ImageMagick.MagickFormat.Tiff64,
            };
        }


        public MagickFormat ToApplicationMagickFormat(ImageMagick.MagickFormat magicMagickFormat)
        {
            return magicMagickFormat switch
            {
                ImageMagick.MagickFormat.Unknown => MagickFormat.Unknown,
                ImageMagick.MagickFormat.Bmp => MagickFormat.Bmp,
                ImageMagick.MagickFormat.Jpeg => MagickFormat.Jpeg,
                ImageMagick.MagickFormat.Jpg => MagickFormat.Jpg,
                ImageMagick.MagickFormat.Png => MagickFormat.Png,
                ImageMagick.MagickFormat.Png00 => MagickFormat.Png00,
                ImageMagick.MagickFormat.Png24 => MagickFormat.Png24,
                ImageMagick.MagickFormat.Png32 => MagickFormat.Png32,
                ImageMagick.MagickFormat.Png48 => MagickFormat.Png48,
                ImageMagick.MagickFormat.Png64 => MagickFormat.Png64,
                ImageMagick.MagickFormat.Png8 => MagickFormat.Png8,
                ImageMagick.MagickFormat.Tif => MagickFormat.Tif,
                ImageMagick.MagickFormat.Tiff => MagickFormat.Tiff,
                ImageMagick.MagickFormat.Tiff64 => MagickFormat.Tiff64,
                _ => throw new InvalidEnumArgumentException($"مقدار جاری ، در نوع {typeof(MagickFormat)} پشتیبانی نمیشود .")
            };
        }


    }
}
