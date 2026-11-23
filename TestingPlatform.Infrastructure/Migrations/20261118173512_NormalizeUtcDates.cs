using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace TestingPlatform.Infrastructure.Migrations;

public partial class NormalizeUtcDates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // The SQLite storage type remains TEXT. Normalize existing offsets to UTC
        // before DateTime comparisons are used by publication and LINQ queries.
        foreach (var (table, column) in new[] {
            ("Users", "CreatedAt"), ("Tests", "CreatedAt"), ("Tests", "PublishedAt"),
            ("Tests", "Deadline"), ("Attempts", "StartedAt"), ("Attempts", "SubmittedAt") })
        {
            migrationBuilder.Sql($"UPDATE \"{table}\" SET \"{column}\" = COALESCE(strftime('%Y-%m-%d %H:%M:%f', \"{column}\"), \"{column}\") WHERE \"{column}\" IS NOT NULL;");
        }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Restore an explicit UTC offset for the old DateTimeOffset reader.
        // The original display offset is not recoverable, but its instant is.
        foreach (var (table, column) in new[] {
            ("Users", "CreatedAt"), ("Tests", "CreatedAt"), ("Tests", "PublishedAt"),
            ("Tests", "Deadline"), ("Attempts", "StartedAt"), ("Attempts", "SubmittedAt") })
        {
            migrationBuilder.Sql($"UPDATE \"{table}\" SET \"{column}\" = COALESCE(strftime('%Y-%m-%d %H:%M:%f', \"{column}\") || '+00:00', \"{column}\") WHERE \"{column}\" IS NOT NULL;");
        }
    }
}
