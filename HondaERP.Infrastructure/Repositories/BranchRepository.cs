using HondaERP.Application.Interfaces;
using HondaERP.Domain.Entities;
using HondaERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HondaERP.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _db;

    public BranchRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Branch>> GetActiveAsync(CancellationToken ct)
    {
        return _db.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    public Task<Branch?> GetActiveByIdAsync(
        int id,
        CancellationToken ct)
    {
        return _db.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsActive,
                ct);
    }
}