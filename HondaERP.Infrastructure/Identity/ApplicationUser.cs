using HondaERP.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HondaERP.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";

    // Owner ki branch null ho sakti hai.
    // Branch staff ko branch assign karein ge.
    public int? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}