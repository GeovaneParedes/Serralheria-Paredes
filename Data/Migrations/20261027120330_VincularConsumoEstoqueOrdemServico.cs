using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Serralheria_Paredes.Data.Migrations
{
    /// <inheritdoc />
    public partial class VincularConsumoEstoqueOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdemServicoId",
                table: "movimentacoes_estoque",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_movimentacoes_estoque_OrdemServicoId",
                table: "movimentacoes_estoque",
                column: "OrdemServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_movimentacoes_estoque_ordens_servico_OrdemServicoId",
                table: "movimentacoes_estoque",
                column: "OrdemServicoId",
                principalTable: "ordens_servico",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movimentacoes_estoque_ordens_servico_OrdemServicoId",
                table: "movimentacoes_estoque");

            migrationBuilder.DropIndex(
                name: "IX_movimentacoes_estoque_OrdemServicoId",
                table: "movimentacoes_estoque");

            migrationBuilder.DropColumn(
                name: "OrdemServicoId",
                table: "movimentacoes_estoque");
        }
    }
}
