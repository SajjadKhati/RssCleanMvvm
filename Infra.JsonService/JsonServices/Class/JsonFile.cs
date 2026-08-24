using System.Text.Json;
using Application.InfrastructureContracts.JsonProject.Json.Interface;

namespace Infra.JsonService.JsonServices.Class
{
    public class JsonFile : IJsonFile
    {

        public TSerialized ReadJson<TSerialized>(string jsonFullFilePath)
        {
            string json = File.ReadAllText(jsonFullFilePath);
            return JsonSerializer.Deserialize<TSerialized>(json);
        }

    }
}
