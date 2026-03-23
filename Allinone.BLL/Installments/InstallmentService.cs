using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Installments;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Installments
{
    public interface IInstallmentService
    {
        Task<IEnumerable<InstallmentDto>> GetAllAsync(bool? isActive = null, InstallmentSortBy sortBy = InstallmentSortBy.UpdatedTime);
        Task<InstallmentSummaryDto> GetSummaryAsync();
        Task<InstallmentScheduleDto> GetScheduleAsync(int id);
        Task<InstallmentDto> Get(int id);
        Task<InstallmentDto> Add(InstallmentAddReq req);
        Task<InstallmentDto> Update(int id, InstallmentAddReq req);
        Task<InstallmentDto> ToggleActiveAsync(int id);
        Task<Installment> Delete(int id);
    }

    public class InstallmentService(
        IInstallmentRepository installmentRepository,
        IMapModel mapper) : BaseBLL, IInstallmentService
    {
        public async Task<IEnumerable<InstallmentDto>> GetAllAsync(bool? isActive = null, InstallmentSortBy sortBy = InstallmentSortBy.UpdatedTime)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entities = await installmentRepository.GetAllByMemberAsync(MemberId, isActive);
            var dtos = entities.Select(MapToDto);

            return sortBy switch
            {
                InstallmentSortBy.Name           => dtos.OrderBy(x => x.Name),
                InstallmentSortBy.RemainingMonths => dtos.OrderBy(x => x.RemainingMonths),
                InstallmentSortBy.Progress       => dtos.OrderByDescending(x => x.ProgressPercentage),
                InstallmentSortBy.MonthlyAmount  => dtos.OrderByDescending(x => x.MonthlyAmount),
                _                                => dtos.OrderByDescending(x => x.UpdatedTime)
            };
        }

        public async Task<InstallmentSummaryDto> GetSummaryAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entities = await installmentRepository.GetAllByMemberAsync(MemberId);
            var dtos = entities.Select(MapToDto).ToList();

            var activeDtos = dtos.Where(x => x.IsActive && !x.IsCompleted).ToList();

            return new InstallmentSummaryDto
            {
                TotalCount = dtos.Count,
                ActiveCount = activeDtos.Count,
                CompletedCount = dtos.Count(x => x.IsCompleted),
                TotalMonthlyCommitment = activeDtos.Sum(x => x.MonthlyAmount),
                TotalRemainingAmount = activeDtos.Sum(x => x.RemainingAmount),
                TotalPaidAmount = dtos.Sum(x => x.PaidAmount),
                Items = dtos
            };
        }

        public async Task<InstallmentScheduleDto> GetScheduleAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await installmentRepository.GetByMemberAsync(MemberId, id)
                ?? throw new InstallmentNotFoundException();

            var now = DateTime.UtcNow.AddHours(8);
            var monthsElapsed = ((now.Year - entity.StartDate.Year) * 12) + (now.Month - entity.StartDate.Month);
            var paidMonths = Math.Min(Math.Max(monthsElapsed + 1, 0), entity.TotalMonths);
            var monthlyAmount = entity.TotalMonths > 0
                ? Math.Round(entity.TotalAmount / entity.TotalMonths, 2)
                : 0;

            var schedule = Enumerable.Range(0, entity.TotalMonths)
                .Select(i => new InstallmentScheduleItem
                {
                    MonthNumber = i + 1,
                    PaymentDate = entity.StartDate.AddMonths(i),
                    Amount = monthlyAmount,
                    IsPaid = i < paidMonths,
                    IsCurrent = i == paidMonths - 1
                })
                .ToList();

            return new InstallmentScheduleDto
            {
                ID = entity.ID,
                Name = entity.Name,
                MonthlyAmount = monthlyAmount,
                TotalMonths = entity.TotalMonths,
                PaidMonths = paidMonths,
                RemainingMonths = entity.TotalMonths - paidMonths,
                Schedule = schedule
            };
        }

        public async Task<InstallmentDto> Get(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await installmentRepository.GetByMemberAsync(MemberId, id)
                ?? throw new InstallmentNotFoundException();

            return MapToDto(entity);
        }

        public async Task<InstallmentDto> Add(InstallmentAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<InstallmentAddReq, Installment>(req);
            entity = ServiceHelper.SetAuditAddMemberDateFields(entity, MemberId);

            await installmentRepository.Add(entity);

            return MapToDto(entity);
        }

        public async Task<InstallmentDto> Update(int id, InstallmentAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await installmentRepository.GetByMemberAsync(MemberId, id)
                ?? throw new InstallmentNotFoundException();

            entity = ServiceHelper.SetAuditUpdateDateFields(entity);
            mapper.Map(req, entity);

            installmentRepository.Update(entity);

            return MapToDto(entity);
        }

        public async Task<InstallmentDto> ToggleActiveAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await installmentRepository.GetByMemberAsync(MemberId, id)
                ?? throw new InstallmentNotFoundException();

            entity.IsActive = !entity.IsActive;
            entity = ServiceHelper.SetAuditUpdateDateFields(entity);

            installmentRepository.Update(entity);

            return MapToDto(entity);
        }

        public async Task<Installment> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await installmentRepository.GetByMemberAsync(MemberId, id)
                ?? throw new InstallmentNotFoundException();

            installmentRepository.Delete(entity);

            return entity;
        }

        private static InstallmentDto MapToDto(Installment entity)
        {
            var now = DateTime.UtcNow.AddHours(8);
            var monthsElapsed = ((now.Year - entity.StartDate.Year) * 12) + (now.Month - entity.StartDate.Month);
            var paidMonths = Math.Min(Math.Max(monthsElapsed + 1, 0), entity.TotalMonths);
            var remainingMonths = entity.TotalMonths - paidMonths;
            var monthlyAmount = entity.TotalMonths > 0
                ? Math.Round(entity.TotalAmount / entity.TotalMonths, 2)
                : 0;
            var paidAmount = Math.Round(monthlyAmount * paidMonths, 2);
            var remainingAmount = Math.Round(entity.TotalAmount - paidAmount, 2);
            var progressPercentage = entity.TotalMonths > 0
                ? Math.Round((decimal)paidMonths / entity.TotalMonths * 100, 1)
                : 0;

            DateTime? upcomingPaymentDate = remainingMonths > 0
                ? entity.StartDate.AddMonths(paidMonths)
                : null;

            int? daysUntilNextPayment = upcomingPaymentDate.HasValue
                ? (int)(upcomingPaymentDate.Value.Date - now.Date).TotalDays
                : null;

            return new InstallmentDto
            {
                ID = entity.ID,
                Name = entity.Name,
                Note = entity.Note,
                TotalAmount = entity.TotalAmount,
                TotalMonths = entity.TotalMonths,
                StartDate = entity.StartDate,
                EndDate = entity.StartDate.AddMonths(entity.TotalMonths - 1),
                IsActive = entity.IsActive,
                MonthlyAmount = monthlyAmount,
                PaidMonths = paidMonths,
                RemainingMonths = remainingMonths,
                PaidAmount = paidAmount,
                RemainingAmount = remainingAmount < 0 ? 0 : remainingAmount,
                ProgressPercentage = progressPercentage,
                IsCompleted = remainingMonths <= 0,
                UpcomingPaymentDate = upcomingPaymentDate,
                DaysUntilNextPayment = daysUntilNextPayment,
                UpdatedTime = entity.UpdatedTime
            };
        }
    }
}
