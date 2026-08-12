using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces
{
    public interface IXmlParser
    {

        IList<IDictionary<string, string>> GetXmlData(XmlParsingConfig xmlParsingConfig);

    }
}
