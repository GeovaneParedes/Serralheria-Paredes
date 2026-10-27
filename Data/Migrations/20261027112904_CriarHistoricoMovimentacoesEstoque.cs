using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Serralheria_Paredes.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriarHistoricoMovimentacoesEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movimentacoes_estoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InsumoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    SaldoAnterior = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    SaldoPosterior = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    Motivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimentacoes_estoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movimentacoes_estoque_insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movimentacoes_estoque_InsumoId_CreatedAt",
                table: "movimentacoes_estoque",
                columns: new[] { "InsumoId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimentacoes_estoque");
        }
    }
}
