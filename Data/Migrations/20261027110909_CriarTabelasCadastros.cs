using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Serralheria_Paredes.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelasCadastros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeOuRazaoSocial = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NomeFantasia = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TipoPessoa = table.Column<int>(type: "integer", nullable: false),
                    CpfOuCnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RgOuInscricaoEstadual = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnderecoCep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EnderecoLogradouro = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EnderecoNumero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EnderecoComplemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnderecoBairro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    EnderecoCidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    EnderecoEstado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RazaoSocial = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NomeFantasia = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TipoPessoa = table.Column<int>(type: "integer", nullable: false),
                    CpfOuCnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InscricaoEstadual = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContatoResponsavel = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    EnderecoCep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EnderecoLogradouro = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EnderecoNumero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EnderecoComplemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnderecoBairro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    EnderecoCidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    EnderecoEstado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fornecedores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clientes_CpfOuCnpj",
                table: "clientes",
                column: "CpfOuCnpj");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_NomeOuRazaoSocial",
                table: "clientes",
                column: "NomeOuRazaoSocial");

            migrationBuilder.CreateIndex(
                name: "IX_fornecedores_CpfOuCnpj",
                table: "fornecedores",
                column: "CpfOuCnpj");

            migrationBuilder.CreateIndex(
                name: "IX_fornecedores_RazaoSocial",
                table: "fornecedores",
                column: "RazaoSocial");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "fornecedores");
        }
    }
}
