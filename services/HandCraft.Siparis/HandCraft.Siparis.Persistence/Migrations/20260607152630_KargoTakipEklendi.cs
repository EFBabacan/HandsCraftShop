using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandCraft.Siparis.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KargoTakipEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KargoKodu",
                table: "Siparisler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KargoKodu",
                table: "Siparisler");
        }
    }
}
