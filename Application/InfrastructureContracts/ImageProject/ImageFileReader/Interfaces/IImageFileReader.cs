namespace Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces
{
    public interface IImageFileReader
    {

        byte[,,] Read8BitImageFromFile(string fullFilePath);

    }
}
