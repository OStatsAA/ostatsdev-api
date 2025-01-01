using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DatasetsUsersAccessLevels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DatasetsProjectsLinks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Datasets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsDeleted",
                table: "Roles",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsDeleted",
                table: "Projects",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetsUsersAccessLevels_IsDeleted",
                table: "DatasetsUsersAccessLevels",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetsProjectsLinks_IsDeleted",
                table: "DatasetsProjectsLinks",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_IsDeleted",
                table: "Datasets",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsDeleted",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Roles_IsDeleted",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Projects_IsDeleted",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_DatasetsUsersAccessLevels_IsDeleted",
                table: "DatasetsUsersAccessLevels");

            migrationBuilder.DropIndex(
                name: "IX_DatasetsProjectsLinks_IsDeleted",
                table: "DatasetsProjectsLinks");

            migrationBuilder.DropIndex(
                name: "IX_Datasets_IsDeleted",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DatasetsUsersAccessLevels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DatasetsProjectsLinks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Datasets");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);
        }
    }
}
