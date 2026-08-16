using Infra.DataAccess.DbContextService.Class;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.Class;

namespace Infra.DataAccess.UnitOfWorkFactoryService.Class
{
    /// دقت کنید که این کلاس چون شی جدیدی از RssDbContext میسازه ، حتما لازم نیست که مسئول Dispose اش ، خودش باشه .
    /// در شرایط خاص مثل UnitOfWork و Query ، ما میتونیم مالکیتِ Disposable کردن را به این سرویس ها تغییر بدیم .
    /// یعنی سرویس Factory شون ، فقط شی DbContext را بسازه اما این سرویس ها ، مسئول Dispose کردنش باشند .
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {

        private string connectionString;


        /////////////////////////////////////////////


        public UnitOfWorkFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }


        /////////////////////////////////////////////


        public IUnitOfWork Create(bool shouldUseLazyLoading = false)
        {
            /// دقت کنید که این کلاس چون شی جدیدی از RssDbContext میسازه ، حتما لازم نیست که مسئول Dispose اش ، خودش باشه .
            /// در شرایط خاص مثل UnitOfWork و Query ، ما میتونیم مالکیتِ Disposable کردن را به این سرویس ها تغییر بدیم .
            /// یعنی سرویس Factory شون ، فقط شی DbContext را بسازه اما این سرویس ها ، مسئول Dispose کردنش باشند .
            return new UnitOfWork(new RssDbContext(this.connectionString, shouldUseLazyLoading));
        }

    }
}
