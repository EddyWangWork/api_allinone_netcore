using Allinone.DLL.Data;
using Allinone.Domain.Auditlogs;

namespace Allinone.DLL.Repositories
{
    public interface IAuditlogRepository : IBaseRepository<Auditlog>
    {
    }

    public class AuditlogRepository(DSContext _context) : BaseRepository<Auditlog>(_context), IAuditlogRepository
    {
    }

}