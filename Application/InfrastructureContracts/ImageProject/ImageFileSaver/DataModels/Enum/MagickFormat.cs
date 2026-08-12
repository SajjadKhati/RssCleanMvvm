namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum
{
    public enum MagickFormat
    {
        Unknown = 1,
        /// <summary>
        /// Microsoft Windows bitmap image.
        /// </summary>
        Bmp,

        /// <summary>
        /// Joint Photographic Experts Group JFIF format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Joint Photographic Experts Group JFIF format.
        /// </summary>
        Jpg,

        /// <summary>
        /// Portable Network Graphics.
        /// </summary>
        Png,

        /// <summary>
        /// PNG inheriting bit-depth and color-type from original.
        /// </summary>
        Png00,

        /// <summary>
        /// opaque or binary transparent 24-bit RGB.
        /// </summary>
        Png24,

        /// <summary>
        /// opaque or transparent 32-bit RGBA.
        /// </summary>
        Png32,

        /// <summary>
        /// opaque or binary transparent 48-bit RGB.
        /// </summary>
        Png48,

        /// <summary>
        /// opaque or transparent 64-bit RGBA.
        /// </summary>
        Png64,

        /// <summary>
        /// 8-bit indexed with optional binary transparency.
        /// </summary>
        Png8,

        /// <summary>
        /// Tagged Image File Format.
        /// </summary>
        Tif,

        /// <summary>
        /// Tagged Image File Format.
        /// </summary>
        Tiff,

        /// <summary>
        /// Tagged Image File Format (64-bit).
        /// </summary>
        Tiff64,
    }
}
