using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#nullable disable
namespace ControleFinanceiro_Backend.Migrations
{
    public partial class AddParcelamentoOrcamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>("parcelado", "Transacoes", "boolean", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<int>("nrParcelas", "Transacoes", "integer", nullable: true);
            migrationBuilder.AddColumn<int>("nrParcelaAtual", "Transacoes", "integer", nullable: true);
            migrationBuilder.AddColumn<int>("cdTransacaoPai", "Transacoes", "integer", nullable: true);

            migrationBuilder.CreateTable(
                name: "Orcamentos",
                columns: table => new
                {
                    cdOrcamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cdUsuario = table.Column<int>(type: "integer", nullable: false),
                    cdCategoria = table.Column<int>(type: "integer", nullable: false),
                    vlLimite = table.Column<decimal>(type: "numeric", nullable: false),
                    nrMes = table.Column<int>(type: "integer", nullable: false),
                    nrAno = table.Column<int>(type: "integer", nullable: false),
                    dtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamentos", x => x.cdOrcamento);
                    table.ForeignKey("FK_Orcamentos_Usuarios", x => x.cdUsuario, "Usuarios", "cdUsuario", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Orcamentos_Categorias", x => x.cdCategoria, "Categorias", "cdCategoria", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex("IX_Orcamentos_cdUsuario_cdCategoria_nrMes_nrAno", "Orcamentos",
                new[] { "cdUsuario", "cdCategoria", "nrMes", "nrAno" }, unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Orcamentos");
            migrationBuilder.DropColumn("parcelado", "Transacoes");
            migrationBuilder.DropColumn("nrParcelas", "Transacoes");
            migrationBuilder.DropColumn("nrParcelaAtual", "Transacoes");
            migrationBuilder.DropColumn("cdTransacaoPai", "Transacoes");
        }
    }
}
