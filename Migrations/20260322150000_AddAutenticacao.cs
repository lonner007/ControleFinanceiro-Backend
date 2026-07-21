using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#nullable disable
namespace ControleFinanceiro_Backend.Migrations
{
    public partial class AddAutenticacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    cdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nmUsuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dsEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    dsSenhaHash = table.Column<string>(type: "text", nullable: false),
                    dsRefreshToken = table.Column<string>(type: "text", nullable: true),
                    dtRefreshTokenExpira = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    dtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Usuarios", x => x.cdUsuario); });

            migrationBuilder.CreateIndex("IX_Usuarios_dsEmail", "Usuarios", "dsEmail", unique: true);

            // Adicionar cdUsuario em todas as tabelas (defaultValue: 1 para registros existentes)
            migrationBuilder.AddColumn<int>("cdUsuario", "Contas", "integer", nullable: false, defaultValue: 1);
            migrationBuilder.AddColumn<int>("cdUsuario", "Categorias", "integer", nullable: false, defaultValue: 1);
            migrationBuilder.AddColumn<int>("cdUsuario", "Metas", "integer", nullable: false, defaultValue: 1);
            migrationBuilder.AddColumn<int>("cdUsuario", "Transacoes", "integer", nullable: false, defaultValue: 1);

            migrationBuilder.AddForeignKey("FK_Contas_Usuarios", "Contas", "cdUsuario", "Usuarios", principalColumn: "cdUsuario", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Categorias_Usuarios", "Categorias", "cdUsuario", "Usuarios", principalColumn: "cdUsuario", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Metas_Usuarios", "Metas", "cdUsuario", "Usuarios", principalColumn: "cdUsuario", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Transacoes_Usuarios", "Transacoes", "cdUsuario", "Usuarios", principalColumn: "cdUsuario", onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex("IX_Contas_cdUsuario", "Contas", "cdUsuario");
            migrationBuilder.CreateIndex("IX_Categorias_cdUsuario", "Categorias", "cdUsuario");
            migrationBuilder.CreateIndex("IX_Metas_cdUsuario", "Metas", "cdUsuario");
            migrationBuilder.CreateIndex("IX_Transacoes_cdUsuario", "Transacoes", "cdUsuario");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Contas_Usuarios", "Contas");
            migrationBuilder.DropForeignKey("FK_Categorias_Usuarios", "Categorias");
            migrationBuilder.DropForeignKey("FK_Metas_Usuarios", "Metas");
            migrationBuilder.DropForeignKey("FK_Transacoes_Usuarios", "Transacoes");
            migrationBuilder.DropColumn("cdUsuario", "Contas");
            migrationBuilder.DropColumn("cdUsuario", "Categorias");
            migrationBuilder.DropColumn("cdUsuario", "Metas");
            migrationBuilder.DropColumn("cdUsuario", "Transacoes");
            migrationBuilder.DropTable("Usuarios");
        }
    }
}
