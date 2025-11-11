using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistroDeTickets.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_usuario",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "pk_usuario",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ticket",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "Id_usuario",
                table: "Ticket",
                newName: "PrioridadId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_Id_usuario",
                table: "Ticket",
                newName: "IX_Ticket_PrioridadId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Usuario",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Usuario",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TokenHashRecuperacion",
                table: "Usuario",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenHashRecuperacionExpiracion",
                table: "Usuario",
                type: "DATETIME2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Ticket",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Ticket",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Ticket",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "Ticket",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Id_cliente",
                table: "Ticket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id_tecnico",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admin_Usuario",
                        column: x => x.Id,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Domicilio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_Usuario",
                        column: x => x.Id,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteTecnico",
                columns: table => new
                {
                    IdReporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "date", nullable: false),
                    IdTicket = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteTecnico", x => x.IdReporte);
                    table.ForeignKey(
                        name: "FK_Ticket",
                        column: x => x.IdTicket,
                        principalTable: "Ticket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tecnico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tecnico_Usuario",
                        column: x => x.Id,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketEstado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketEstado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketPrioridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPrioridad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Id",
                table: "Usuario",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Username",
                table: "Usuario",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_EstadoId",
                table: "Ticket",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Id_cliente",
                table: "Ticket",
                column: "Id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Id_tecnico",
                table: "Ticket",
                column: "Id_tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteTecnico_IdTicket",
                table: "ReporteTecnico",
                column: "IdTicket");

            migrationBuilder.CreateIndex(
                name: "UQ_TicketEstado_Nombre",
                table: "TicketEstado",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_TicketPrioridad_Nombre",
                table: "TicketPrioridad",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Cliente",
                table: "Ticket",
                column: "Id_cliente",
                principalTable: "Cliente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Estado",
                table: "Ticket",
                column: "EstadoId",
                principalTable: "TicketEstado",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Prioridad",
                table: "Ticket",
                column: "PrioridadId",
                principalTable: "TicketPrioridad",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Tecnico",
                table: "Ticket",
                column: "Id_tecnico",
                principalTable: "Tecnico",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Cliente",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Estado",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Prioridad",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Tecnico",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "ReporteTecnico");

            migrationBuilder.DropTable(
                name: "Tecnico");

            migrationBuilder.DropTable(
                name: "TicketEstado");

            migrationBuilder.DropTable(
                name: "TicketPrioridad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_Email",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_Id",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_Username",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_EstadoId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_Id_cliente",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_Id_tecnico",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenHashRecuperacion",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenHashRecuperacionExpiracion",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Id_cliente",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Id_tecnico",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "PrioridadId",
                table: "Ticket",
                newName: "Id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_PrioridadId",
                table: "Ticket",
                newName: "IX_Ticket_Id_usuario");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Usuario",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuario",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Ticket",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Ticket",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Ticket",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Ticket",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Ticket",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Ticket",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ticket",
                table: "Ticket",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_usuario",
                table: "Ticket",
                column: "Id_usuario",
                principalTable: "Usuario",
                principalColumn: "Id");
        }
    }
}
