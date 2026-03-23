using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Installments
{
    public class InstallmentAddReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "TotalAmount must be greater than 0")]
        public decimal TotalAmount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TotalMonths must be at least 1")]
        public int TotalMonths { get; set; }

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Note { get; set; }
    }

    public class InstallmentDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int PaidMonths { get; set; }
        public int RemainingMonths { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    public class InstallmentSummaryDto
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int CompletedCount { get; set; }
        public decimal TotalMonthlyCommitment { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public IEnumerable<InstallmentDto> Items { get; set; }
    }
}
