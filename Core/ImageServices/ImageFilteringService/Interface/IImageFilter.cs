namespace Core.ImageServices.ImageFilteringService.Interface
{
    public interface IImageFilter
    {
        byte[,,] ToGrayScale(byte[,,] argb8bitImage);
    }
}
