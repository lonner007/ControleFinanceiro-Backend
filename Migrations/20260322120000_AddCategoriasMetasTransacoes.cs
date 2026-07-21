using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ControleFinanceiro_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriasMetasTransacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    cdCategoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nmCategoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dsCategoria = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tpCategoria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    icone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    dtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.cdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Metas",
                columns: table => new
                {
                    cdMeta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nmMeta = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    dsMeta = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    vlAlvo = table.Column<decimal>(type: "numeric", nullable: false),
                    vlAtual = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    dtPrazo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    dtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metas", x => x.cdMeta);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                columns: table => new
                {
                    cdTransacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tpTransacao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cdConta = table.Column<int>(type: "integer", nullable: false),
                    cdContaDestino = table.Column<int>(type: "integer", nullable: true),
                    cdCategoria = table.Column<int>(type: "integer", nullable: true),
                    vlTransacao = table.Column<decimal>(type: "numeric", nullable: false),
                    dtTransacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dsTransacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    dtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.cdTransacao);
                    table.ForeignKey(
                        name: "FK_Transacoes_Contas_cdConta",
                        column: x => x.cdConta,
                        principalTable: "Contas",
                        principalColumn: "cdConta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacoes_Contas_cdContaDestino",
                        column: x => x.cdContaDestino,
                        principalTable: "Contas",
                        principalColumn: "cdConta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacoes_Categorias_cdCategoria",
                        column: x => x.cdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "cdCategoria",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_cdConta",
                table: "Transacoes",
                column: "cdConta");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_cdContaDestino",
                table: "Transacoes",
                column: "cdContaDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_cdCategoria",
                table: "Transacoes",
                column: "cdCategoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Transacoes");
            migrationBuilder.DropTable(name: "Metas");
            migrationBuilder.DropTable(name: "Categorias");
        }
    }
}
