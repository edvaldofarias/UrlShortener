using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable


namespace UrlShortener.WebApi.Infra.Migrations
{

    [ExcludeFromCodeCoverage]
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "Shorten_HashId_Seq",
                startValue: 15000000L);

            migrationBuilder.CreateTable(
                name: "Shorten",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LongUrl = table.Column<string>(type: "text", nullable: false),
                    HashId = table.Column<long>(type: "bigint", nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shorten", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shorten_HashId",
                table: "Shorten",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shorten_ShortCode",
                table: "Shorten",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shorten");

            migrationBuilder.DropSequence(
                name: "Shorten_HashId_Seq");
        }
    }
}
