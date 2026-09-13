using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;

namespace Application.InfrastructureContracts.SharedProject.ImageFileExtensionResolver.Interface
{
    public interface IImageFileExtensionResolver
    {
        string GetFileExtension(MagickFormat magickFormat);
    }
}
