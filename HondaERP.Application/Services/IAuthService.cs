using HondaERP.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace HondaERP.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
