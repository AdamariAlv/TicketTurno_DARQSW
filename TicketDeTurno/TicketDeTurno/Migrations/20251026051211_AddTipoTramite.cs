using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketDeTurno.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoTramite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposTramite",
                columns: table => new
                {
                    TipoTramiteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTramite", x => x.TipoTramiteId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TiposTramite");
        }
    }
}
