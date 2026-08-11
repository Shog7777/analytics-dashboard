namespace AnalyticsDashboard.Api.Common;

/// <summary>Role names used in [Authorize] attributes, kept in one place instead of magic strings.</summary>
public static class Roles
{
    public const string Viewer = "Viewer";
    public const string Editor = "Editor";
    public const string Admin = "Admin";

    public const string EditorOrAdmin = Editor + "," + Admin;
    public const string AnyRole = Viewer + "," + Editor + "," + Admin;
}
