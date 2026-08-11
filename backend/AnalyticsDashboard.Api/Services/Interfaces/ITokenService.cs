using AnalyticsDashboard.Api.Models;

namespace AnalyticsDashboard.Api.Services.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
