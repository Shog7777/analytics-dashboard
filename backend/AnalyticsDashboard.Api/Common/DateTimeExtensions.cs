namespace AnalyticsDashboard.Api.Common;

public static class DateTimeExtensions
{
    /// <summary>
    /// Marks a DateTime as UTC without changing its value. Needed because Npgsql rejects
    /// Kind=Unspecified dates on timestamptz columns, and that's what query-string/JSON
    /// dates come in as. We treat all API date input as UTC.
    /// </summary>
    public static DateTime AsUtc(this DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    public static DateTime? AsUtc(this DateTime? value) =>
        value.HasValue ? value.Value.AsUtc() : null;
}
