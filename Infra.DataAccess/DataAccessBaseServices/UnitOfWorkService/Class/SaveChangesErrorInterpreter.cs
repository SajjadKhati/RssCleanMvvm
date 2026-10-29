using Infra.DataAccess.EfEntities.Class;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.Class
{
    internal class SaveChangesErrorInterpreter
    {

        private const int MaxIndexKeyLength = 449;


        /////////////////////////////////////////////////////////////////////////////


        internal bool IsIndexKeyTooLong(DbUpdateException ex)
        {
            if (ex.InnerException is not SqlException sqlEx) return false;
            if (sqlEx.Number == 1946) return true;
            return sqlEx.Message.Contains("exceeds the maximum length", StringComparison.OrdinalIgnoreCase)
                   && sqlEx.Message.Contains("clustered", StringComparison.OrdinalIgnoreCase);
        }


        internal IList<string> ExtractTooLongRssItemIds(DbUpdateException ex)
        {
            return ex.Entries
                .Select(e => e.Entity)
                .OfType<RssItemEfEntity>()
                .Select(e => e.Id)
                .Where(id => !string.IsNullOrEmpty(id) && id.Length > MaxIndexKeyLength)
                .ToList()!;
        }

    }
}
