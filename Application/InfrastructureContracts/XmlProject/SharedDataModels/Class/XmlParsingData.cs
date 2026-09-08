namespace Application.InfrastructureContracts.XmlProject.SharedDataModels.Class
{
    public class XmlParsingData : XmlParsingConfig
    {

        public string XmlData {  get; set; }


        ////////////////////////////////////////////////////////////////
        

        public XmlParsingData()
        {

        }

        public XmlParsingData(string xmlData, string parentElementName, IList<string> childElementNames, IList<string> attributeNames = null) :
            base(parentElementName, childElementNames, attributeNames)
        {
            this.XmlData = xmlData;
        }

        public XmlParsingData(string xmlData, XmlParsingConfig config) : base(config)
        {
            this.XmlData = xmlData;
        }

    }
}
