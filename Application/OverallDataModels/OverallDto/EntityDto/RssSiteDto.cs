namespace Application.OverallDataModels.OverallDto.EntityDto
{
    public class RssSiteDto
    {

        public string Id { get; set; }

        public IList<RssFeedDto> RssFeedDtos { get; set; }

        public string SiteName { get; set; }

    }
}
