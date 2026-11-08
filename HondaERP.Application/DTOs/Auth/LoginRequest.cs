using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HondaERP.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }


    public class LoginResponse
    {
        public string AccessToken { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }

        public string FullName { get; set; } = "";
        public int? BranchId { get; set; }
        public string[] Roles { get; set; } = [];
    }
}
