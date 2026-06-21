using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "user_password", "user_status" },
                values: new object[] { "$2a$12$8b5.j/Yj6EwG.Gl.x0zame3GUA7PGid9N1/twuLSPFAuk/b9wR5Me", "A" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "user_password", "user_status" },
                values: new object[] { "$2a$12$gjZJ.srEyPnLQOd926FweOccmMHKVw82fZKboGl0ylQnqbB4KaMoq", "\0" });
        }
    }
}
