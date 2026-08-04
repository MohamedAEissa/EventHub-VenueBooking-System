using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTicketsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTicket_AspNetUsers_ClientId",
                table: "UserTicket");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTicket_Tickets_TicketId",
                table: "UserTicket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTicket",
                table: "UserTicket");

            migrationBuilder.RenameTable(
                name: "UserTicket",
                newName: "UserTickets");

            migrationBuilder.RenameIndex(
                name: "IX_UserTicket_TicketId",
                table: "UserTickets",
                newName: "IX_UserTickets_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTicket_ClientId",
                table: "UserTickets",
                newName: "IX_UserTickets_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTickets",
                table: "UserTickets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTickets_AspNetUsers_ClientId",
                table: "UserTickets",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTickets_Tickets_TicketId",
                table: "UserTickets",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTickets_AspNetUsers_ClientId",
                table: "UserTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTickets_Tickets_TicketId",
                table: "UserTickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTickets",
                table: "UserTickets");

            migrationBuilder.RenameTable(
                name: "UserTickets",
                newName: "UserTicket");

            migrationBuilder.RenameIndex(
                name: "IX_UserTickets_TicketId",
                table: "UserTicket",
                newName: "IX_UserTicket_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTickets_ClientId",
                table: "UserTicket",
                newName: "IX_UserTicket_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTicket",
                table: "UserTicket",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTicket_AspNetUsers_ClientId",
                table: "UserTicket",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTicket_Tickets_TicketId",
                table: "UserTicket",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
