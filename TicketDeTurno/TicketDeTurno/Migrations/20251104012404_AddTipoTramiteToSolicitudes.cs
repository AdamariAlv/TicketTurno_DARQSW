using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketDeTurno.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoTramiteToSolicitudes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoTramiteId",
                table: "Solicitudes",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_TipoTramiteId",
                table: "Solicitudes",
                column: "TipoTramiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_TiposTramite_TipoTramiteId",
                table: "Solicitudes",
                column: "TipoTramiteId",
                principalTable: "TiposTramite",
                principalColumn: "TipoTramiteId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_TiposTramite_TipoTramiteId",
                table: "Solicitudes");

            migrationBuilder.DropIndex(
                name: "IX_Solicitudes_TipoTramiteId",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "TipoTramiteId",
                table: "Solicitudes");
        }
    }
}
