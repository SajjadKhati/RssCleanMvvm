using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.AppQueryService.RssSiteQueryService.Class
{
    public class RssSiteQuery : DbContextOwningQueryBase, IRssSiteQuery
    {

        private RssSiteEfEntityMapper rssSiteEfEntityMapper;


        ////////////////////////////////////////////////////////////////////////


        public RssSiteQuery(RssDbContext dbContext, RssSiteEfEntityMapper rssSiteEfEntityMapper) : base(dbContext)
        {
            this.rssSiteEfEntityMapper = rssSiteEfEntityMapper;
        }


        ////////////////////////////////////////////////////////////////////////
        
        
        public IList<RssSiteDto> FindAllEagerLoading()
        {
            return this.FindAllEntity<RssSiteEfEntity>()?
                .Include(rssSiteEfEntity => rssSiteEfEntity.RssFeedEfEntities)?
                .ToList()?
                .Select(rssSiteEfEntity => this.rssSiteEfEntityMapper.ToRssSiteDto(rssSiteEfEntity))?
                .ToList();
        }

    }
}
