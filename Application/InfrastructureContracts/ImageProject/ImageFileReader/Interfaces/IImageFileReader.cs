namespace Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces
{
    public interface IImageFileReader
    {

        byte[,,] ReadArgb8BitImageFromFile(string fullFilePath);

    }
}
