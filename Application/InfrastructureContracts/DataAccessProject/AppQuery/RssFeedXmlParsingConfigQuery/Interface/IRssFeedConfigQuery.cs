using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface
{
    public interface IRssFeedConfigQuery : IDisposable
    {

        IReadOnlyDictionary<Uri, RssFeedConfig> FindAll();

    }
}
