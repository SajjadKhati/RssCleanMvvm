namespace Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels
{
    public class RssItemDataEditableInput
    {

        public Uri Id { get; set; }

        public string Description { get; set; }


        /////////////////////////////////////////////////////////////////////////


        public RssItemDataEditableInput()
        {

        }


        public RssItemDataEditableInput(Uri id, string description)
        {
            this.Id = id;
            this.Description = description;
        }

    }
}
