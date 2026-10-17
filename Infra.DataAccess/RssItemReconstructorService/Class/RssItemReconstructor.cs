using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.RssItemReconstructorService.Interface;

namespace Infra.DataAccess.RssItemReconstructorService.Class
{
    public class RssItemReconstructor : IRssItemReconstructor
    {

        private IImageFileReader imageFileReader;

        private RssItemEfEntityMapper efEntityMapper;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemReconstructor(IImageFileReader imageFileReader, RssItemEfEntityMapper efEntityMapper)
        {
            this.imageFileReader = imageFileReader;
            this.efEntityMapper = efEntityMapper;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItem Reconstruct(RssItemEfEntity rssItemEfEntity)
        {
            byte[,,] imageData = this.imageFileReader.Read8BitImageFromFile(rssItemEfEntity.ImageFullFilePath);
            return this.efEntityMapper.ToRssItem(rssItemEfEntity, imageData);
        }

    }
}
