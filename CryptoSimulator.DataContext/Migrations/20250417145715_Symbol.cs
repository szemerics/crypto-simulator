using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoSimulator.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class Symbol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "CryptoCurrencies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "CryptoCurrencies");
        }
    }
}
