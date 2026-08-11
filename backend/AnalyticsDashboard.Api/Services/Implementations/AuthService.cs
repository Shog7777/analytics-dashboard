using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.Data;
using AnalyticsDashboard.Api.DTOs.Auth;
using AnalyticsDashboard.Api.Models;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await _db.Users.AnyAsync(u =>
            u.Username == dto.Username || u.Email == dto.Email);

        if (exists)
        {
            throw ApiException.Conflict("A user with this username or email already exists.");
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            // First user ever registered becomes Admin so the system is never left
            // without an administrator; everyone after that starts as Viewer.
            Role = await _db.Users.AnyAsync() ? UserRole.Viewer : UserRole.Admin
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw ApiException.BadRequest("Invalid username/email or password.");
        }

        return BuildAuthResponse(user);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _db.Users
            .OrderBy(u => u.Username)
            .Select(u => new UserDto(u.Id, u.Username, u.Email, u.Role.ToString()))
            .ToListAsync();
    }

    public async Task<UserDto> UpdateUserRoleAsync(int userId, string role)
    {
        if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
        {
            throw ApiException.BadRequest($"'{role}' is not a valid role. Use Viewer, Editor or Admin.");
        }

        var user = await _db.Users.FindAsync(userId)
            ?? throw ApiException.NotFound(nameof(User), userId);

        user.Role = parsedRole;
        await _db.SaveChangesAsync();

        return new UserDto(user.Id, user.Username, user.Email, user.Role.ToString());
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id, user.Username, user.Email, user.Role.ToString());
        return new AuthResponseDto(token, expiresAt, userDto);
    }
}
