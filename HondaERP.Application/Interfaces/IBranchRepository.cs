using HondaERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HondaERP.Application.Interfaces
{
    public interface IBranchRepository
    {
        Task<List<Branch>> GetActiveAsync(CancellationToken ct);

        Task<Branch?> GetActiveByIdAsync(int id, CancellationToken ct);
    }
}
