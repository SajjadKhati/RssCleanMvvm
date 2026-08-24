namespace Application.InfrastructureContracts.JsonProject.Json.Interface
{
    public interface IJsonFile
    {

        TSerialized ReadJson<TSerialized>(string jsonFullFilePath);

    }
}
