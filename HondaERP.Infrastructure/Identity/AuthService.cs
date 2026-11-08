using HondaERP.Application.DTOs.Auth;
using HondaERP.Application.Services;
using Jose;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HondaERP.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly JwtOptions _jwt;

    public AuthService(
        UserManager<ApplicationUser> users,
        IOptions<JwtOptions> jwt)
    {
        _users = users;
        _jwt = jwt.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _users.FindByEmailAsync(request.Email.Trim());

        if (user is null || !user.IsActive)
            return null;

        if (await _users.IsLockedOutAsync(user))
            return null;

        var passwordValid = await _users.CheckPasswordAsync(
            user,
            request.Password);

        if (!passwordValid)
        {
            await EnsureSuccessAsync(_users.AccessFailedAsync(user));
            return null;
        }

        await EnsureSuccessAsync(
            _users.ResetAccessFailedCountAsync(user));

        var roles = await _users.GetRolesAsync(user);

        // Branch staff ke liye active assigned branch required.
        if (!roles.Contains("Owner"))
        {
            if (user.BranchId is null)
                return null;
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("name", user.FullName)
        };

        foreach (var role in roles)
            claims.Add(new Claim("role", role));

        if (user.BranchId.HasValue)
        {
            claims.Add(new Claim(
                "branchId",
                user.BranchId.Value.ToString()));
        }

        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwt.Key)),
                SecurityAlgorithms.HmacSha256));

        return new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler()
                .WriteToken(token),
            ExpiresAtUtc = expires,
            FullName = user.FullName,
            BranchId = user.BranchId,
            Roles = roles.ToArray()
        };
    }

    private static async Task EnsureSuccessAsync(
        Task<IdentityResult> operation)
    {
        var result = await operation;

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(
                "; ",
                result.Errors.Select(x => x.Description)));
        }
    }
}