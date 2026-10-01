using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeeltekoolV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class languagecoursetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nimetus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kirjeldus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageCourses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageCourses");
        }
    }
}
