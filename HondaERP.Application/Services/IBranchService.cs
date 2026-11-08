using HondaERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HondaERP.Application.Services
{
    public interface IBranchService
    {
        Task<List<BranchDto>> GetAllAsync(CancellationToken ct);

        Task<BranchDto?> GetByIdAsync(int id, CancellationToken ct);
    }
}
