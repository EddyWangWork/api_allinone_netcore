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
    }

    public class InstallmentDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalMonths { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int PaidMonths { get; set; }
        public int RemainingMonths { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
