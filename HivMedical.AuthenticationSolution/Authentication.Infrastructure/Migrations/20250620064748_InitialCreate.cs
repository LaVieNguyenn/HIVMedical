using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "users");

            migrationBuilder.DropColumn(
                name: "allergies",
                table: "users");

            migrationBuilder.DropColumn(
                name: "blood_type",
                table: "users");

            migrationBuilder.DropColumn(
                name: "chronic_conditions",
                table: "users");

            migrationBuilder.DropColumn(
                name: "city",
                table: "users");

            migrationBuilder.DropColumn(
                name: "country",
                table: "users");

            migrationBuilder.DropColumn(
                name: "emergency_contact_name",
                table: "users");

            migrationBuilder.DropColumn(
                name: "emergency_contact_phone",
                table: "users");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "users");

            migrationBuilder.DropColumn(
                name: "state",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 1,
                columns: new[] { "created_at", "created_by", "IsDeleted", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 20, 6, 47, 47, 600, DateTimeKind.Utc).AddTicks(7595), null, false, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 2,
                columns: new[] { "created_at", "created_by", "IsDeleted", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 20, 6, 47, 47, 600, DateTimeKind.Utc).AddTicks(7596), null, false, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 3,
                columns: new[] { "created_at", "created_by", "IsDeleted", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 20, 6, 47, 47, 600, DateTimeKind.Utc).AddTicks(7598), null, false, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 4,
                columns: new[] { "created_at", "created_by", "IsDeleted", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 20, 6, 47, 47, 600, DateTimeKind.Utc).AddTicks(7599), null, false, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 5,
                columns: new[] { "created_at", "created_by", "IsDeleted", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 20, 6, 47, 47, 600, DateTimeKind.Utc).AddTicks(7600), null, false, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 6,
                columns: new[] { "created_at", "created_by", "IsDeleted", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 20, 6, 47, 47, 600, DateTimeKind.Utc).AddTicks(7601), null, false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "roles");

            migrationBuilder.AlterColumn<int>(
                name: "updated_by",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "created_by",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "allergies",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blood_type",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chronic_conditions",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emergency_contact_name",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emergency_contact_phone",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "updated_by",
                table: "roles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "created_by",
                table: "roles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 1,
                columns: new[] { "created_at", "created_by", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8748), null, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 2,
                columns: new[] { "created_at", "created_by", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8751), null, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 3,
                columns: new[] { "created_at", "created_by", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8752), null, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 4,
                columns: new[] { "created_at", "created_by", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8753), null, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 5,
                columns: new[] { "created_at", "created_by", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8754), null, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 6,
                columns: new[] { "created_at", "created_by", "updated_by" },
                values: new object[] { new DateTime(2025, 6, 17, 17, 43, 11, 763, DateTimeKind.Utc).AddTicks(8754), null, null });
        }
    }
}
