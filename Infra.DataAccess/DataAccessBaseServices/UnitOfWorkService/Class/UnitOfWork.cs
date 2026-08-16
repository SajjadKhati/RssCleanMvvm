using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.InternalInterface;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.Class
{
    public class UnitOfWork : IUnitOfWork, IDbContextProvider
    {

        private DbContext _dbContext;

        private bool isAlreadyDisposed;


        //////////////////////////////////////////


        public DbContext DbContext
        {
            get 
            {
                if(this.isAlreadyDisposed == true)
                    throw new ObjectDisposedException(nameof(this.DbContext), "این شی قبلا Dispose شده است .");

                return this._dbContext; 
            }
            init
            {
                this._dbContext = value;
            }
        }


        //////////////////////////////////////////


        public UnitOfWork(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.isAlreadyDisposed = false;
        }


        //////////////////////////////////////////


        public int SaveChanges()
        {
            return this.DbContext.SaveChanges();
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (this.isAlreadyDisposed == false)
            {
                if (disposing == true)
                {
                    this.DbContext?.Dispose();
                }

                this.isAlreadyDisposed = true;
            }
        }

    }
}
