namespace Application.OverallDataModels.OverallDto.EntityDto
{
    public class RssItemDto
    {

        public string Id { get; set; }

        public string Link { get; set; }

        public string Category { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime PublishDate { get; set; }

        public string PersianPublishDate { get; set; }

        public string ImageFullFilePath { get; set; }

        public bool IsUnsaved { get; set; }


        /////////////////////////////////////////////////////////////////////////

        public RssItemDto()
        {
        }

        public RssItemDto(RssItemDto rssItemDto)
        {
            this.Id = rssItemDto.Id;
            this.Link = rssItemDto.Link;
            this.Category = rssItemDto.Category;
            this.Title = rssItemDto.Title;
            this.Description = rssItemDto.Description;
            this.PublishDate = rssItemDto.PublishDate;
            this.PersianPublishDate = rssItemDto.PersianPublishDate;
            this.ImageFullFilePath = rssItemDto.ImageFullFilePath;
            this.IsUnsaved = rssItemDto.IsUnsaved;
        }

        public RssItemDto(RssItemDto rssItemDto, bool isUnsaved) : this(rssItemDto)
        {
            this.IsUnsaved = isUnsaved;
        }

    }
}
