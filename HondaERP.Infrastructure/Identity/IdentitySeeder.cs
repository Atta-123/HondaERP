using Microsoft.AspNetCore.Identity;

namespace HondaERP.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            "Owner",
            "Manager",
            "Cashier",
            "Mechanic"
        };

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var result = await roleManager.CreateAsync(
                new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Could not create role '{role}': {errors}");
            }
        }
    }



    public static async Task SeedOwnerAsync(
    UserManager<ApplicationUser> userManager,
    string email,
    string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "Abdul Rashid Afridi",
                IsActive = true,
                BranchId = null,
                LockoutEnabled = true
            };

            var created = await userManager.CreateAsync(user, password);

            if (!created.Succeeded)
            {
                throw new InvalidOperationException(string.Join(
                    "; ",
                    created.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, "Owner"))
        {
            var assigned = await userManager.AddToRoleAsync(user, "Owner");

            if (!assigned.Succeeded)
            {
                throw new InvalidOperationException(string.Join(
                    "; ",
                    assigned.Errors.Select(x => x.Description)));
            }
        }
    }
}