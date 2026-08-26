namespace Application.InfrastructureContracts.XmlProject.SharedDataModels.Class
{
    public class XmlParsingConfig
    {

        public string ParentElementName { get; set; }

        public IList<string> ChildElementNames { get; set; }

        public IList<string> AttributeNames { get; set; } = null;


        ////////////////////////////////////////////////////////////////


        public XmlParsingConfig()
        {

        }


        public XmlParsingConfig(string parentElementName, IList<string> childElementNames, IList<string> attributeNames = null)
        {
            this.ParentElementName = parentElementName;
            this.ChildElementNames = childElementNames;
            this.AttributeNames = attributeNames;
        }


        public XmlParsingConfig(XmlParsingConfig xmlParsingConfig)
        {
            this.ParentElementName = xmlParsingConfig.ParentElementName;
            this.ChildElementNames = xmlParsingConfig.ChildElementNames;
            this.AttributeNames = xmlParsingConfig.AttributeNames;
        }

    }
}
