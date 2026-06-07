using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandCraft.Siparis.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SiparisDurumEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "Siparisler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Siparisler");
        }
    }
}
