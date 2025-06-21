using Allinone.Domain;
using Allinone.Domain.Audits;

namespace Allinone.BLL
{
    public class ServiceHelper
    {
        public static T SetAuditAddMemberFields<T>(T entity, int memberId) where T : IAuditableMember
        {
            entity.MemberID = memberId;
            return entity;
        }

        public static T SetAddMemberFields<T>(T entity, int memberId) where T : IMember
        {
            entity.MemberID = memberId;
            return entity;
        }

        public static T SetAuditAddMemberDateFields<T>(T entity, int memberId) where T : IAuditableMemberUpdatedTime
        {
            entity.MemberID = memberId;
            entity.UpdatedTime = DateTime.UtcNow.AddHours(8);
            return entity;
        }

        public static T SetAuditUpdateDateFields<T>(T entity) where T : IAuditableMemberUpdatedTime
        {
            entity.UpdatedTime = DateTime.UtcNow.AddHours(8);
            return entity;
        }

        public static List<T> EnsureNotNullOrEmpty<T>(IEnumerable<T>? source, Exception exceptionToThrow)
        {
            if (source?.Any() != true)
                throw exceptionToThrow;

            return [.. source];
        }

        public static T EnsureNotNull<T>(T? source, Exception exceptionToThrow) where T : class
        {
            return source ?? throw exceptionToThrow;
        }
    }
}
