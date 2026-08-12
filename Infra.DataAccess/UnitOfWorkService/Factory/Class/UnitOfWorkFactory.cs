using Infra.DataAccess.UnitOfWorkService.Class;
using Infra.DataAccess.DbContextService.Class;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Factory.Interfaces;

namespace Infra.DataAccess.UnitOfWorkService.Factory.Class
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {

        private string connectionString;


        /////////////////////////////////////////////


        public UnitOfWorkFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }


        /////////////////////////////////////////////


        public IUnitOfWork Create(bool shouldUseLazyLoading)
        {
            return new UnitOfWork(new RssDbContext(this.connectionString, shouldUseLazyLoading));
        }

    }
}
