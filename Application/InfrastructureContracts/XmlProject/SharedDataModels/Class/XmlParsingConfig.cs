namespace Application.InfrastructureContracts.XmlProject.SharedDataModels.Class
{
    public class XmlParsingConfig
    {

        public string xmlData {  get; set; }

        public string ParentElementName { get; set; }

        public IList<string> ChildElementNames { get; set; }

        public IList<string> AttributeNames { get; set; } = null;


        ////////////////////////////////////////////////////////////////
        

        public XmlParsingConfig()
        {

        }


        public XmlParsingConfig(string xmlData, string parentElementName, IList<string> childElementNames, IList<string> attributeNames = null)
        {
            this.xmlData = xmlData;
            this.ParentElementName = parentElementName;
            this.ChildElementNames = childElementNames;
            this.AttributeNames = attributeNames;
        }

    }
}
