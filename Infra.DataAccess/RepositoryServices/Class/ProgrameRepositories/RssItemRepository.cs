using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.RepositoryServices.Class.RepositoryBases;
using Application.InfrastructureContracts.CacheProject.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;

namespace Infra.DataAccess.RepositoryServices.Class.ProgrameRepositories
{
    public class RssItemRepository : RepositoryBase, IRssItemRepository
    {

        private ICache<Uri, RssItemEfEntity> persistanceCache;

        private RssItemEfEntityMapper efEntityMapper;

        private IImageFileSaverCoordinator imageFileSaverCoordinator;
        
        private IImageFileReader imageFileReader;


        ///////////////////////////////////////////////////////////////


        public RssItemRepository(RssDbContext rssDbContext, ICache<Uri, RssItemEfEntity> persistanceCache, RssItemEfEntityMapper efEntityMapper,
            IImageFileSaverCoordinator imageFileSaverCoordinator, IImageFileReader imageFileReader) : base(rssDbContext)
        {
            this.persistanceCache = persistanceCache;
            this.efEntityMapper = efEntityMapper;
            this.imageFileSaverCoordinator = imageFileSaverCoordinator;
            this.imageFileReader = imageFileReader;
        }


        ///////////////////////////////////////////////////////////////


        public void Add(RssItem rssItem, string imageFolderPath, ImageFileSettings imageFileSettings)
        {
            DestinationPath imageDestinationPath = this.SaveImageToFile(imageFolderPath, rssItem.Id.AbsoluteUri, imageFileSettings);
            RssItemEfEntity rssItemEfEntity = this.efEntityMapper.ToRssItemEfEntity(rssItem, imageDestinationPath.FullFilePath);
            this.AddEntity(rssItemEfEntity);
        }


        public void Delete(RssItem rssItem)
        {
            RssItemEfEntity rssItemEfEntity = this.FindRssItemEfEntityByKey(rssItem.Id.AbsoluteUri);
            this.DeleteEntity(rssItemEfEntity);
        }


        public void Update(RssItem rssItem, string imageFolderPath, ImageFileSettings imageFileSettings)
        {
            DestinationPath imageDestinationPath = this.SaveImageToFile(imageFolderPath, rssItem.Id.AbsoluteUri, imageFileSettings);
            RssItemEfEntity oldRssItemEfEntity = this.FindRssItemEfEntityByKey(rssItem.Id.AbsoluteUri);
            this.AttachEntityToDbContext(oldRssItemEfEntity);
            /// به روزرسانی
            RssItemEfEntity updatedRssItemEfEntity = this.efEntityMapper.ToRssItemEfEntity(rssItem, imageDestinationPath.FullFilePath);
            this.efEntityMapper.UpdateRssItemEfEntity(updatedRssItemEfEntity, oldRssItemEfEntity);
        }


        public RssItem FindByKey(string rssItemKey)
        {
            RssItemEfEntity rssItemEfEntity = this.FindRssItemEfEntityByKey(rssItemKey);
            return this.ReconstructionRssItem(rssItemEfEntity);
        }


        public IEnumerable<RssItem> FindAll()
        {
            IQueryable<RssItemEfEntity> rssItemEfEntityQuery = this.FindAllEntity<RssItemEfEntity>();
            List<RssItemEfEntity> rssItemEfEntities = rssItemEfEntityQuery.ToList();
            return this.ReconstructionRssItems(rssItemEfEntities);

        }


        ////////////////////////////////////


        private RssItemEfEntity FindRssItemEfEntityByKey(string rssItemKey)
        {
            return this.FindByKeyEntity<string, RssItemEfEntity>(rssItemKey);
        }


        private DestinationPath SaveImageToFile(string imageFolderPath, string imageFileName, ImageFileSettings imageFileSettings)
        {
            var imageFilePath = new FilePath(imageFolderPath, imageFileName);
            DestinationPath imageDestinationPath = this.imageFileSaverCoordinator.Save8BitImageToFile(imageFileSettings, imageFilePath);
            return imageDestinationPath;
        }


        private IEnumerable<RssItem> ReconstructionRssItems(IEnumerable<RssItemEfEntity> rssItemEfEntities)
        {
            List<RssItem> rssItems = new List<RssItem>();
            foreach (RssItemEfEntity rssItemEfEntity in rssItemEfEntities)
            {
                RssItem rssItem = this.ReconstructionRssItem(rssItemEfEntity);
                if (rssItem != null)
                    rssItems.Add(rssItem);
            }
            return (rssItems.Count > 0) ? rssItems : null;
        }


        private RssItem ReconstructionRssItem(RssItemEfEntity rssItemEfEntity)
        {
            byte[,,] imageData = this.imageFileReader.Read8BitImageFromFile(rssItemEfEntity.ImageFullFilePath);
            return this.efEntityMapper.ToRssItem(rssItemEfEntity, imageData);
        }


    }
}
