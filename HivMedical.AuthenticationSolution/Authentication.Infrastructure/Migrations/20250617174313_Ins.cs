using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Authentication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Ins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gender = table.Column<byte>(type: "tinyint", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_anonymous = table.Column<bool>(type: "bit", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    state = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    postal_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    emergency_contact_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    emergency_contact_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    blood_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chronic_conditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "created_at", "created_by", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8748), null, "Guest", null, null },
                    { 2, new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8751), null, "Customer", null, null },
                    { 3, new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8752), null, "Staff", null, null },
                    { 4, new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8753), null, "Doctor", null, null },
                    { 5, new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8754), null, "Manager", null, null },
                    { 6, new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8754), null, "Admin", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
