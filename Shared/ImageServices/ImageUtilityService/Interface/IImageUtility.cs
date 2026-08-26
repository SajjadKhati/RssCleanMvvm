namespace Shared.ImageServices.ImageUtilityService.Interface
{
    public interface IImageUtility
    {

        byte[] FlattenImageData(byte[,,] imageData, int height, int width, int channels);

    }
}
