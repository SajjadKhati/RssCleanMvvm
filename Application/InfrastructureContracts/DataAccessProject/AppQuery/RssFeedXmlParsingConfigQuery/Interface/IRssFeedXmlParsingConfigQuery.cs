using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface
{
    public interface IRssFeedXmlParsingConfigQuery : IDisposable
    {

        IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig> FindAll();

    }
}
