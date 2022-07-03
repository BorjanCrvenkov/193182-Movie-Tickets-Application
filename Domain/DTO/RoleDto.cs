using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTO
{
    public class RoleDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}
