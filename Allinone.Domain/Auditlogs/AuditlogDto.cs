using Allinone.Domain.Audits;
using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Auditlogs
{
    public class AuditlogDto : Auditlog
    {
        public string TypeName { get; set; }
        public string ActionTypeName { get; set; }
    }
}
