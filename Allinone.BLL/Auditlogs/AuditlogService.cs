using Allinone.DLL.Repositories;
using Allinone.Domain.Auditlogs;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops;
using Allinone.Helper.Datetimes;
using Allinone.Helper.Mapper;
using AutoMapper;

namespace Allinone.BLL.Auditlogs
{
    public interface IAuditlogService
    {
        Task<IEnumerable<AuditlogDto>> GetAllByMemberAsync();
        Task<Auditlog> GetAllByMemberAsync(int id);
        Task<Auditlog> AddAsync(Auditlog req);

        Task<Auditlog> LogLoginNew(string name, int memberid);

        Task<Auditlog> LogShopDiaryNew(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogShopDiaryUpdate(string name, int memberid, string reqNew = "", string reqOld = "");

        Task<Auditlog> LogTodolistDoneNew(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogTodolistDoneUpdate(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogTodolistDoneDelete(string name, int memberid, string reqNew = "", string reqOld = "");

        Task<Auditlog> LogTodolistNew(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogTodolistUpdate(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogTodolistDelete(string name, int memberid, string reqNew = "", string reqOld = "");

        Task<Auditlog> LogShopNew(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogShopUpdate(string name, int memberid, string reqNew = "", string reqOld = "");
        Task<Auditlog> LogShopDelete(string name, int memberid, string reqNew = "", string reqOld = "");
    }

    public class AuditlogService(
        IAuditlogRepository _auditlogRepository,
        IMapModel _mapper) : BaseBLL, IAuditlogService
    {

        public async Task<IEnumerable<AuditlogDto>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var auditlogs = await _auditlogRepository.GetAllByMemberAsync(MemberId);
            var auditlogsEntities = auditlogs.Select(x => _mapper.MapDto<AuditlogDto>(x));
            return auditlogsEntities.OrderByDescending(x => x.CreatedTime);
        }

        public async Task<Auditlog> GetAllByMemberAsync(int id)
        {
            return await _auditlogRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryActivityNotFoundException();
        }

        public async Task<Auditlog> AddAsync(Auditlog req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        /*------------------- Shop Diary*/
        public async Task<Auditlog> LogShopDiaryUpdate(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.ShopDiary,
                ActionTypeID = (int)EnumAuditlogActionType.Update,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogShopDiaryNew(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.ShopDiary,
                ActionTypeID = (int)EnumAuditlogActionType.New,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        /*------------------- Shop */
        public async Task<Auditlog> LogShopDelete(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Shop,
                ActionTypeID = (int)EnumAuditlogActionType.Delete,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogShopUpdate(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Shop,
                ActionTypeID = (int)EnumAuditlogActionType.Update,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogShopNew(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Shop,
                ActionTypeID = (int)EnumAuditlogActionType.New,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        /*------------------- TodolistDone */
        public async Task<Auditlog> LogTodolistDoneNew(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.TodolistDone,
                ActionTypeID = (int)EnumAuditlogActionType.Done,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogTodolistDoneUpdate(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.TodolistDone,
                ActionTypeID = (int)EnumAuditlogActionType.Update,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogTodolistDoneDelete(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.TodolistDone,
                ActionTypeID = (int)EnumAuditlogActionType.Delete,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        /*------------------- Todolist */
        public async Task<Auditlog> LogTodolistDelete(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Todolist,
                ActionTypeID = (int)EnumAuditlogActionType.Delete,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogTodolistUpdate(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Todolist,
                ActionTypeID = (int)EnumAuditlogActionType.Update,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        public async Task<Auditlog> LogTodolistNew(
            string name, int memberid, string reqNew = "", string reqOld = "")
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Todolist,
                ActionTypeID = (int)EnumAuditlogActionType.New,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name,
                OldValue = reqOld,
                NewValue = reqNew
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }

        /*------------------- Login */
        public async Task<Auditlog> LogLoginNew(string name, int memberid)
        {
            Auditlog req = new()
            {
                TypeID = (int)EnumAuditlogType.Login,
                ActionTypeID = (int)EnumAuditlogActionType.Done,
                CreatedTime = DatetimeHelper.UTC8Now(),
                MemberID = memberid,
                Name = name
            };

            await _auditlogRepository.AddAsync(req);

            return req;
        }
    }
}
