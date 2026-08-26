using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Interface;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntityMapperService.Class;

namespace Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory
{
    /// دقت کنید که این کلاس چون شی جدیدی از RssDbContext میسازه ، حتما لازم نیست که مسئول Dispose اش ، خودش باشه .
    /// در شرایط خاص مثل UnitOfWork و Query ، ما میتونیم مالکیتِ Disposable کردن را به این سرویس ها تغییر بدیم .
    /// یعنی سرویس Factory شون ، فقط شی DbContext را بسازه اما این سرویس ها ، مسئول Dispose کردنش باشند .
    public class RssSiteQueryFactory : IRssSiteQueryFactory
    {

        private string connectionString;

        private RssSiteEfEntityMapper RssSiteEfEntityMapper;

        /////////////////////////////////////////////////////////////////


        public RssSiteQueryFactory(string connectionString, RssSiteEfEntityMapper RssSiteEfEntityMapper)
        {
            this.connectionString = connectionString;
            this.RssSiteEfEntityMapper = RssSiteEfEntityMapper;
        }


        /////////////////////////////////////////////////////////////////


        /// دقت کنید که این کلاس چون شی جدیدی از RssDbContext میسازه ، حتما لازم نیست که مسئول Dispose اش ، خودش باشه .
        /// در شرایط خاص مثل UnitOfWork و Query ، ما میتونیم مالکیتِ Disposable کردن را به این سرویس ها تغییر بدیم .
        /// یعنی سرویس Factory شون ، فقط شی DbContext را بسازه اما این سرویس ها ، مسئول Dispose کردنش باشند .
        public IRssSiteQuery Create(bool shouldUseLazyLoading = false)
        {
            return new RssSiteQuery(new RssDbContext(this.connectionString, shouldUseLazyLoading), this.RssSiteEfEntityMapper);
        }

    }
}
