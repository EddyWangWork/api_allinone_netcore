using Allinone.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Todolists
{
    public class TodolistAddReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "Name cannot exceed 20 characters")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [EnumDataType(typeof(EnumTodolistType), ErrorMessage = "CategoryId should correct")]
        public int CategoryId { get; set; }
    }
}
