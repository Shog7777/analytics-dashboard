namespace AnalyticsDashboard.Api.Models;

/// <summary>Viewer reads, Editor manages content, Admin has full control.</summary>
public enum UserRole
{
    Viewer = 0,
    Editor = 1,
    Admin = 2
}
