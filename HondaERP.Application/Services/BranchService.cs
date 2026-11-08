using HondaERP.Application.DTOs;
using HondaERP.Application.Interfaces;

using HondaERP.Domain.Entities;

namespace HondaERP.Application.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _repository;

    public BranchService(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BranchDto>> GetAllAsync(
        CancellationToken ct)
    {
        var branches = await _repository.GetActiveAsync(ct);

        return branches.Select(MapToDto).ToList();
    }

    public async Task<BranchDto?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var branch = await _repository.GetActiveByIdAsync(id, ct);

        return branch is null ? null : MapToDto(branch);
    }

    private static BranchDto MapToDto(Branch branch)
    {
        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Address = branch.Address,
            Phone = branch.Phone
        };
    }
}