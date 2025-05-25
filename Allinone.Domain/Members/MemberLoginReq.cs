using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Members
{
    public class MemberLoginReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "Name cannot exceed 20 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, ErrorMessage = "Password cannot exceed 20 characters")]
        public string? Password { get; set; }
    }
}
