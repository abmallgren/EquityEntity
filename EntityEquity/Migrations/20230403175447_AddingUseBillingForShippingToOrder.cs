using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityEquity.Migrations
{
    public partial class AddingUseBillingForShippingToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatformFee",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Headers",
                table: "Emails",
                newName: "From");

            migrationBuilder.AddColumn<bool>(
                name: "UseBillingAddressForShipping",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseBillingAddressForShipping",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "Emails",
                newName: "Headers");

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFee",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
